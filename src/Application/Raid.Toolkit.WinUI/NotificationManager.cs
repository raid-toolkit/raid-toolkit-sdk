using Microsoft.Extensions.Hosting;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Windows.AppNotifications;

using Raid.Toolkit.Extensibility;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Raid.Toolkit.UI.WinUI
{
    public class ToastNotification : INotification
    {
        private readonly string Title;
        private readonly string Message;
        private readonly string Action;
        public string ScenarioId { get; set; } = String.Empty;

        public ToastNotification(string title, string message, string action)
        {
            Title = title;
            Message = message;
            Action = action;
        }

        public string GetXml()
        {
            return new ToastContentBuilder()
                .AddArgument(NotificationConstants.ScenarioId, ScenarioId)
                .AddArgument("action", Action)
                .AddText(Title)
                .AddText(Message)
                .GetXml().GetXml();
        }
    }

    public class NotificationSink : INotificationSink
    {
        private readonly NotificationManager NotificationManager;
        private readonly string ScenarioId;
        private bool IsDisposed;

        public event EventHandler<NotificationActivationEventArgs>? Activated;

        public NotificationSink(NotificationManager notificationManager, string scenarioId)
        {
            NotificationManager = notificationManager;
            ScenarioId = scenarioId;
        }

        public void SendNotification(INotification appNotification)
        {
            appNotification.ScenarioId = ScenarioId;
            AppNotification toast = new(appNotification.GetXml());
            AppNotificationManager.Default.Show(toast);
        }

        internal void Handle(IReadOnlyDictionary<string, string> args, IReadOnlyDictionary<string, string> inputs)
        {
            Activated?.Invoke(this, new NotificationActivationEventArgs(args, inputs));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    NotificationManager.UnregisterHandler(ScenarioId);
                }

                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class NotificationManager : INotificationManager, IHostedService
    {
        private volatile bool IsRegistered;

        private readonly ConcurrentDictionary<string, NotificationSink> NotificationHandlers = new();

        public NotificationManager()
        {
            IsRegistered = false;
        }

        ~NotificationManager()
        {
            Unregister();
        }

        public INotificationSink RegisterHandler(string scenarioId)
        {
            NotificationSink sink = new(this, scenarioId);
            if (!NotificationHandlers.TryAdd(scenarioId, sink))
                throw new ArgumentException("Scenario already has a handler registered", nameof(scenarioId));

            return sink;
        }

        public bool UnregisterHandler(string scenarioId)
        {
            return NotificationHandlers.TryRemove(scenarioId, out _);
        }

        public void Initialize()
        {
            AppNotificationManager notificationManager = AppNotificationManager.Default;

            // To ensure all Notification handling happens in this process instance, register for
            // NotificationInvoked before calling Register(). Without this a new process will
            // be launched to handle the notification.
            notificationManager.NotificationInvoked += OnNotificationInvoked;

            notificationManager.Register();
            IsRegistered = true;
        }

        public void Unregister()
        {
            if (IsRegistered)
            {
                AppNotificationManager.Default.Unregister();
                IsRegistered = false;
            }
        }

        public bool DispatchNotification(AppNotificationActivatedEventArgs notificationActivatedEventArgs)
        {
            IReadOnlyDictionary<string, string> args = notificationActivatedEventArgs.Argument
                .Split(';')
                .Select(v => v.Split('='))
                .ToDictionary(kvp => kvp[0], kvp => kvp[1]);
            Dictionary<string, string> inputs = new(notificationActivatedEventArgs.UserInput);
            if (!args.TryGetValue(NotificationConstants.ScenarioId, out string? scenarioId)
                || string.IsNullOrEmpty(scenarioId)
                || !NotificationHandlers.TryGetValue(scenarioId, out NotificationSink? sink)
                || sink == null)
            {
                return false;
            }
            try
            {
                sink.Handle(args, inputs);
            }
            catch
            {
                // TODO: Log error
                return false;
            }
            return true;
        }

        void OnNotificationInvoked(object sender, AppNotificationActivatedEventArgs notificationActivatedEventArgs)
        {
            if (!DispatchNotification(notificationActivatedEventArgs))
            {
                // TODO: Log lossy notification
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Unregister();
            return Task.CompletedTask;
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.WinUI.Notifications;

using Il2CppToolkit.Common.Errors;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Windows.AppNotifications;

using Raid.Toolkit.Common;

using Windows.Foundation.Metadata;

namespace Raid.Toolkit.Extensibility.Notifications
{
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

        public AppNotification SendNotification(ToastContent content)
        {
            AppNotification toast = new(content.GetContent());
            AppNotificationManager.Default.Show(toast);
            return toast;
        }

        public string GetArguments(string action) => GetArguments(action, new Dictionary<string, string>());
        public string GetArguments(string action, IReadOnlyDictionary<string, string> args)
        {
            Dictionary<string, string> kvps = args != null ? new(args) : new();
            kvps.Add(NotificationConstants.ScenarioId, ScenarioId);
            kvps.Add(NotificationConstants.Action, action);
            return string.Join(';', kvps.Select(kvp => $"{kvp.Key}={kvp.Value}"));
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
                    _ = NotificationManager.UnregisterHandler(ScenarioId);
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

        private readonly ILogger<NotificationManager> Logger;
        private readonly ConcurrentDictionary<string, object> NotificationHandlers = new();

        public NotificationManager(ILogger<NotificationManager> logger)
        {
            IsRegistered = false;
            Logger = logger;
            Initialize();
        }

        ~NotificationManager()
        {
            Unregister();
        }

        public INotificationSink RegisterHandler(string scenarioId)
        {
            NotificationSink sink = new(this, scenarioId);
            NotificationHandlers.AddOrUpdate(scenarioId, sink, (scenarioId, entry) =>
            {
                if (entry is AppNotificationActivatedEventArgs eventArgs)
                {
                    DispatchNotificationArgs(sink, eventArgs);
                }
                else
                {
                    throw new ArgumentException("Scenario already has a handler registered", nameof(scenarioId));
                }
                return sink;
            });
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

        private static void DispatchNotificationArgs(NotificationSink sink, AppNotificationActivatedEventArgs notificationActivatedEventArgs)
        {
            string arg = notificationActivatedEventArgs.Argument;
            IReadOnlyDictionary<string, string> args = ParseArgs(ref arg);
            Dictionary<string, string> inputs = new(notificationActivatedEventArgs.UserInput);
            sink.Handle(args, inputs);
        }

        public bool HandleNotificationActivated(AppNotificationActivatedEventArgs notificationActivatedEventArgs)
        {
            string arg = notificationActivatedEventArgs.Argument;
            IReadOnlyDictionary<string, string> args = ParseArgs(ref arg);
            if (!args.TryGetValue(NotificationConstants.ScenarioId, out string? scenarioId) || string.IsNullOrEmpty(scenarioId))
            {
                return false;
            }
            try
            {
                if (!NotificationHandlers.TryGetValue(scenarioId, out object? entry))
                {
                    if (!NotificationHandlers.TryAdd(scenarioId, notificationActivatedEventArgs))
                    {
                        throw new ApplicationException("Duplicate NotificationActivatedEvent is unexpected");
                    }
                }
                else if (entry is NotificationSink sink)
                {
                    DispatchNotificationArgs(sink, notificationActivatedEventArgs);
                }
                else if (entry is AppNotificationActivatedEventArgs)
                {
                    throw new ApplicationException("Duplicate NotificationActivatedEvent is unexpected");
                }
                else
                {
                    throw new ApplicationException($"Unexpected value type {entry.GetType().Name} in NotificationHandlers");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(HostError.NotificationHandlerError.EventId(), ex, "An exception occurred handling notification activation");
                return false;
            }
            return true;
        }

        private void OnNotificationInvoked(object sender, AppNotificationActivatedEventArgs notificationActivatedEventArgs)
        {
            if (!HandleNotificationActivated(notificationActivatedEventArgs))
            {
                Logger.LogError(HostError.NotificationHandlerNotFound.EventId(), "Notification handler not found");
            }
        }

        private static IReadOnlyDictionary<string, string> ParseArgs(ref string arg)
        {
            if (!arg.Contains('=')) // not kvp?
            {
                arg = $"{NotificationConstants.Action}={arg}";
            }
            IReadOnlyDictionary<string, string> args = arg
                .Split(';')
                .Select(v => v.Split('='))
                .ToDictionary(kvp => kvp[0], kvp => kvp[1]);
            return args;
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

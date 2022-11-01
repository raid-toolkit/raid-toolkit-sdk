using CommunityToolkit.WinUI.Notifications;

using Microsoft.Windows.AppNotifications;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Raid.Toolkit.Extensibility
{
    public interface INotification
    {
        string ScenarioId { get; set; }
        string GetXml();
    }
    public class NotificationActivationEventArgs : EventArgs
    {
        public NotificationActivationEventArgs(IReadOnlyDictionary<string, string> arguments, IReadOnlyDictionary<string, string> inputs)
        {
            Arguments = arguments;
            Inputs = inputs;
        }

        public IReadOnlyDictionary<string, string> Arguments { get; }
        public IReadOnlyDictionary<string, string> Inputs { get; }
    }

    public interface INotificationSink : IDisposable
    {
        public event EventHandler<NotificationActivationEventArgs> Activated;
        AppNotification SendNotification(ToastContent notification);
    }
    public interface INotificationManager
    {
        void Initialize();
        INotificationSink RegisterHandler(string scenarioId);
    }
    public static class NotificationConstants
    {
        public static string ScenarioId => "scenarioId";
        public static string Action => "action";
    }
}

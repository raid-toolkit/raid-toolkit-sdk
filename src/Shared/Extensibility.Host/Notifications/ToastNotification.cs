using CommunityToolkit.WinUI.Notifications;

using System;

namespace Raid.Toolkit.Extensibility.Notifications
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
                .Content.GetContent();
        }
    }
}

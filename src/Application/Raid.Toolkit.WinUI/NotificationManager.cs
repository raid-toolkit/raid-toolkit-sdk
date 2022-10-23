using Microsoft.Windows.AppNotifications;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.UI.WinUI
{
    public class NotificationManager
    {
        private bool m_isRegistered;

        private readonly Dictionary<int, Action<AppNotificationActivatedEventArgs>> c_notificationHandlers = new();

        public NotificationManager()
        {
            m_isRegistered = false;
        }

        ~NotificationManager()
        {
            Unregister();
        }

        public void Init()
        {
            var notificationManager = AppNotificationManager.Default;

            // To ensure all Notification handling happens in this process instance, register for
            // NotificationInvoked before calling Register(). Without this a new process will
            // be launched to handle the notification.
            notificationManager.NotificationInvoked += OnNotificationInvoked;

            notificationManager.Register();
            m_isRegistered = true;
        }

        public void Unregister()
        {
            if (m_isRegistered)
            {
                AppNotificationManager.Default.Unregister();
                m_isRegistered = false;
            }
        }

        void OnNotificationInvoked(object sender, AppNotificationActivatedEventArgs notificationActivatedEventArgs)
        {
            //NotifyUser.NotificationReceived();

            //if (!DispatchNotification(notificationActivatedEventArgs))
            //{
            //    NotifyUser.UnrecognizedToastOriginator();
            //}
        }
    }
}

using System;

namespace Raid.Toolkit.Extensibility.Host.Services
{
    public class SessionManager : ISessionManager
    {
        public int SessionCount { get; private set; }
        private DateTime LastSessionDisconnect;
        public DateTime LastSessionActive => SessionCount > 0 ? DateTime.UtcNow : LastSessionDisconnect;

        public void OnClosed()
        {
            LastSessionDisconnect = DateTime.UtcNow;
            --SessionCount;
        }

        public void OnConnected()
        {
            ++SessionCount;
        }
    }
}
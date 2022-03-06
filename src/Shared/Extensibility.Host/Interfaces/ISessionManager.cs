using System;

namespace Raid.Toolkit.Extensibility.Host
{
    public interface ISessionManager
    {
        int SessionCount { get; }
        DateTime LastSessionActive { get; }
        void OnConnected();
        void OnClosed();
    }
}

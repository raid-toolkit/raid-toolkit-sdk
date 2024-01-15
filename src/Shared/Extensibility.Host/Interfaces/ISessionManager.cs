using System;

namespace Raid.Toolkit.Extensibility.Host;

[Obsolete("Not supported after 3.0")]
public interface ISessionManager
{
	int SessionCount { get; }
	DateTime LastSessionActive { get; }
	void OnConnected();
	void OnClosed();
}

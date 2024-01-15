using CommunityToolkit.WinUI.Notifications;

using Microsoft.Windows.AppNotifications;

using System;
using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility;

[Obsolete("Not intended for public use")]
public interface INotification
{
	string ScenarioId { get; set; }
	string GetXml();
}

[Obsolete("Not intended for public use")]
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

[Obsolete("Not intended for public use")]
public interface INotificationSink : IDisposable
{
	string ScenarioId { get; }
	event EventHandler<NotificationActivationEventArgs> Activated;
	string GetArguments(string action);
	string GetArguments(string action, IReadOnlyDictionary<string, string> args);
	AppNotification SendNotification(ToastContent notification, string? tag = null);
	void Handle(NotificationActivationEventArgs eventArgs);
}

[Obsolete("Not intended for public use")]
public interface INotificationManager
{
	void Initialize();
	void RegisterHandler(INotificationSink sink);
}

[Obsolete("Not intended for public use")]
public static class NotificationConstants
{
	public static string ScenarioId => "scenarioId";
	public static string Action => "action";
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.WinUI.Notifications;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;

using Windows.ApplicationModel.Activation;

namespace Raid.Toolkit.Extensibility.Notifications;

public class NotificationSink : INotificationSink
{
	internal NotificationManager? NotificationManager { get; set; }
	public string ScenarioId { get; }
	private bool IsDisposed;

	public event EventHandler<NotificationActivationEventArgs>? Activated;

	public NotificationSink(string scenarioId)
	{
		ScenarioId = scenarioId;
	}

	public AppNotification SendNotification(ToastContent content, string? tag = null)
	{
		AppNotification toast = new(content.GetContent());
		if (!string.IsNullOrEmpty(tag))
		{
			toast.Tag = tag;
		}
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

	public void Handle(NotificationActivationEventArgs eventArgs)
	{
		Activated?.Invoke(this, eventArgs);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			if (disposing)
			{
				NotificationManager?.UnregisterHandler(ScenarioId);
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
	}

	~NotificationManager()
	{
		Unregister();
	}

	public void RegisterHandler(INotificationSink sink)
	{
		NotificationHandlers.AddOrUpdate(sink.ScenarioId, sink, (scenarioId, entry) =>
		{
			if (entry is NotificationActivationEventArgs eventArgs)
			{
				DispatchNotificationArgs(sink, eventArgs);
			}
			else
			{
				throw new ArgumentException("Scenario already has a handler registered", nameof(scenarioId));
			}
			return sink;
		});
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
		AppActivationArguments? appActivationArgs = AppInstance.GetCurrent()?.GetActivatedEventArgs();
		if (appActivationArgs != null)
		{
			if (appActivationArgs.Kind == ExtendedActivationKind.ToastNotification && appActivationArgs.Data is IToastNotificationActivatedEventArgs toastNotificationActivatedEventArgs)
			{
				if (!HandleNotificationActivated(ConvertArgs(toastNotificationActivatedEventArgs)))
				{
					Logger.LogError(HostError.NotificationHandlerNotFound.EventId(), "Notification handler not found");
				}
			}
			else if (appActivationArgs.Kind == ExtendedActivationKind.AppNotification && appActivationArgs.Data is AppNotificationActivatedEventArgs appNotificationActivatedEventArgs)
			{
				if (!HandleNotificationActivated(ConvertArgs(appNotificationActivatedEventArgs)))
				{
					Logger.LogError(HostError.NotificationHandlerNotFound.EventId(), "Notification handler not found");
				}
			}
		}
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

	private static void DispatchNotificationArgs(INotificationSink sink, NotificationActivationEventArgs eventArgs)
	{
		sink.Handle(eventArgs);
	}

	public bool HandleNotificationActivated(NotificationActivationEventArgs eventArgs)
	{
		if (!eventArgs.Arguments.TryGetValue(NotificationConstants.ScenarioId, out string? scenarioId) || string.IsNullOrEmpty(scenarioId))
		{
			return false;
		}
		try
		{
			if (!NotificationHandlers.TryGetValue(scenarioId, out object? entry))
			{
				if (!NotificationHandlers.TryAdd(scenarioId, eventArgs))
				{
					throw new ApplicationException("Duplicate NotificationActivatedEvent is unexpected");
				}
			}
			else if (entry is NotificationSink sink)
			{
				DispatchNotificationArgs(sink, eventArgs);
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
		if (!HandleNotificationActivated(ConvertArgs(notificationActivatedEventArgs)))
		{
			Logger.LogError(HostError.NotificationHandlerNotFound.EventId(), "Notification handler not found");
		}
	}

	private static NotificationActivationEventArgs ConvertArgs(AppNotificationActivatedEventArgs notificationActivatedEventArgs)
	{
		string arg = notificationActivatedEventArgs.Argument;
		IReadOnlyDictionary<string, string> args = ParseArgs(ref arg);
		IReadOnlyDictionary<string, string> inputs = new Dictionary<string, string>(notificationActivatedEventArgs.UserInput);
		NotificationActivationEventArgs eventArgs = new(args, inputs);
		return eventArgs;
	}

	private static NotificationActivationEventArgs ConvertArgs(IToastNotificationActivatedEventArgs toastNotificationActivatedEventArgs)
	{
		string arg = toastNotificationActivatedEventArgs.Argument;
		IReadOnlyDictionary<string, string> args = ParseArgs(ref arg);
		IReadOnlyDictionary<string, string> inputs = toastNotificationActivatedEventArgs.UserInput.ToDictionary(k => k.Key, v => v.Value.ToString()!);
		NotificationActivationEventArgs eventArgs = new(args, inputs);
		return eventArgs;
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

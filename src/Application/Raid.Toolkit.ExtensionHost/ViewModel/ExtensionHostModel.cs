using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Helpers;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Extensibility.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinUIEx;

namespace Raid.Toolkit.ExtensionHost.ViewModel;

public class ExtensionHostModel : AsyncObservable
{
	private readonly DependencySynthesizer Dependencies;

	public IPackageManager PackageManager => Dependencies.GetRequiredService<IPackageManager>();
	public ExtensionBundle? BundleToInstall;

	public ExtensionHostUIState State { get => GetField<ExtensionHostUIState>(); private set => SetField(value); }
	public Window? MainWindow { get; internal set; }
	public string ProgressMessage { get => GetField<string>() ?? string.Empty; private set => SetField(value); }
	public string? ProgressStepMessage { get => GetField<string>(); private set => SetField(value); }
	public double Progress { get => GetField<double?>() ?? 0.0; private set => SetField(value); }
	[DependsOn(nameof(Progress))] public bool ProgressIndeterminate => Progress == -1.0;
	public bool ProgressSuccess { get => GetField<bool>() == true; private set => SetField(value); }

	public ExtensionHostModel() : base(App.Current.Dispatcher)
	{
		Dependencies = new(App.Current.ServiceProvider);
	}

	TaskCompletionSource? UserTrustSource;
	public Task RequestUserTrust(ExtensionBundle bundle)
	{
		if (MainWindow == null)
			throw new InvalidOperationException("Window is not set");

		BundleToInstall = bundle;
		Show(ExtensionHostUIState.RequiresTrust);
		UserTrustSource = new();
		return UserTrustSource.Task;
	}

	public void UserTrustResponse(bool trusted)
	{
		if (UserTrustSource == null)
			throw new InvalidOperationException();

		if (trusted)
			UserTrustSource.SetResult();
		else
			UserTrustSource.SetCanceled();
	}

	public void StartProgress(string message)
	{
		ProgressMessage = message;
		ProgressStepMessage = string.Empty;
		Progress = 0;
		State = ExtensionHostUIState.Progress;
	}

	public void UpdateProgress(double? progress, string? stepMessage)
	{
		Progress = progress ?? -1.0;
		ProgressStepMessage = stepMessage;
	}

	public void EndProgress(bool success)
	{
		ProgressSuccess = success;
		ProgressStepMessage = string.Empty;
		Progress = 100;
	}

	public void Show(ExtensionHostUIState state)
	{
		State = state;
		MainWindow?.Show();
	}

	public void Hide()
	{
		MainWindow?.Hide();
	}
}

public enum ExtensionHostUIState
{
	None = 0,
	RequiresTrust,
	Progress,
	About,
}

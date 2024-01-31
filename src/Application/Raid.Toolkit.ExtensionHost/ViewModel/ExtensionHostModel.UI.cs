using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

using WinUIEx;

namespace Raid.Toolkit.ExtensionHost.ViewModel;

public partial class ExtensionHostModel : AsyncObservable
{
    public ExtensionHostUIState State { get => GetField<ExtensionHostUIState>(); private set { SetField(value); StateChanged(); } }
    public ExtensionHostProgressState ProgressState { get => GetField<ExtensionHostProgressState>(); private set => SetField(value); }
    public WindowEx? MainWindow { get; internal set; }
    public string ProgressMessage { get => GetField<string>() ?? string.Empty; private set => SetField(value); }
    public string? ProgressStepMessage { get => GetField<string>(); private set => SetField(value); }
    public double Progress { get => GetField<double?>() ?? 0.0; private set => SetField(value); }
    [DependsOn(nameof(ProgressState))] public bool ProgressIndeterminate => ProgressState == ExtensionHostProgressState.Indeterminate;
    [DependsOn(nameof(ProgressState))] public bool ProgressSuccess => ProgressState == ExtensionHostProgressState.Success;
    [DependsOn(nameof(ProgressState))] public bool ProgressRunning => ProgressState == ExtensionHostProgressState.Running;
    [DependsOn(nameof(ProgressState))] public bool ProgressError => ProgressState == ExtensionHostProgressState.Error;

    public void StateChanged()
    {
        if (MainWindow == null) return;
        Dispatcher.Dispatch(() =>
        {
            switch (State)
            {
                case ExtensionHostUIState.RequiresTrust:
                    MainWindow.CenterOnScreen(400, 400);
                    break;
                case ExtensionHostUIState.Progress:
                    MainWindow.CenterOnScreen(400, 200);
                    break;
                case ExtensionHostUIState.About:
                    MainWindow.CenterOnScreen(350, 475);
                    break;
            }
        });
    }

    public void ReduceWindowHeight(double height)
    {
        if (MainWindow == null) return;
        MainWindow.Height -= height;
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
        ProgressState = ExtensionHostProgressState.Running;
        ProgressMessage = message;
        ProgressStepMessage = string.Empty;
        Progress = 0;
        State = ExtensionHostUIState.Progress;
    }

    public void UpdateProgress(double? progress, string? stepMessage)
    {
        Progress = progress ?? 0;
        ProgressState = progress == null ? ExtensionHostProgressState.Indeterminate : ExtensionHostProgressState.Running;
        ProgressStepMessage = stepMessage;
    }

    public void EndProgress(bool success, string message, string stepMessage)
    {
        ProgressState = success ? ExtensionHostProgressState.Success : ExtensionHostProgressState.Error;
        ProgressMessage = message;
        ProgressStepMessage = stepMessage;
        Progress = 100;
    }

    public void Show(ExtensionHostUIState state)
    {
        State = state;
        Dispatcher.Dispatch(() => MainWindow?.Show());
    }

    public void Hide()
    {
        Dispatcher.Dispatch(() => MainWindow?.Hide());
    }

    public void Close()
    {
        Dispatcher.Dispatch(() => MainWindow?.Close());
    }

    private void Loader_OnStateUpdated(object? sender, ModelLoaderEventArgs e)
    {
        switch (e.LoadState)
        {
            case ModelLoaderState.Initialize:
                UpdateProgress(20, "Initializing model...");
                break;
            case ModelLoaderState.Rebuild:
                UpdateProgress(20, "Rebuilding model...");
                break;
            case ModelLoaderState.Ready:
                UpdateProgress(90, "Model ready.");
                break;
            case ModelLoaderState.Loaded:
                UpdateProgress(95, "Model loaded.");
                break;
            case ModelLoaderState.Error:
                UpdateProgress(null, "An error occurred during installation");
                break;
            default:
                break;
        }
    }
}

public enum ExtensionHostUIState
{
    None = 0,
    RequiresTrust,
    Progress,
    About,
}

public enum ExtensionHostProgressState
{
    None = 0,
    Running,
    Indeterminate,
    Success,
    Error,
}

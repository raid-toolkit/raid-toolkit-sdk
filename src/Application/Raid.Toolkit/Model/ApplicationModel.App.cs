
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raid.Toolkit.UI.WinUI;

namespace Raid.Toolkit.Model;

public interface IApplicationModel
{
	void OnActivated(StartupOptions options);
	void OnLaunched();
	Task WaitForStop();
	void Restart(bool postUpdate, bool asAdmin = false, IWin32Window? owner = null);
	void Exit();
}

public partial class ApplicationModel : AsyncObservable, IApplicationModel
{
	private readonly IServiceProvider ServiceProvider;
	private readonly Queue<StartupOptions> ActivationRequests = new();
	private readonly ILogger Logger;

	public IPackageWorkerManager PackageWorkerManager => ServiceProvider.GetRequiredService<IPackageWorkerManager>();
	public IPackageManager PackageManager => ServiceProvider.GetRequiredService<IPackageManager>();
	public IAppDispatcher Dispatcher => ServiceProvider.GetRequiredService<IAppDispatcher>();
	public IModelLoader ModelLoader => ServiceProvider.GetRequiredService<IModelLoader>();

	public ApplicationModel(IServiceProvider serviceProvider, ILogger<IApplicationModel> logger)
		: base(serviceProvider.GetRequiredService<IAppDispatcher>())
	{
		ServiceProvider = serviceProvider;
		Logger = logger;
	}

	public async void OnLaunched()
	{
		MainWindow = ActivatorUtilities.CreateInstance<MainWindow>(ServiceProvider);

		await ModelLoader.BuildAndLoad(Array.Empty<Regex>(), Path.Join(Path.GetDirectoryName(Environment.ProcessPath), "bin"));

		await PackageWorkerManager.StartExtensions();

		while (ActivationRequests.TryDequeue(out StartupOptions? options))
			DoActivation(options);
	}

	public void OnActivated(StartupOptions options)
	{
		// defer activation processing until after the main window has been created
		if (MainWindow != null)
		{
			DoActivation(options);
		}
		else
		{
			ActivationRequests.Enqueue(options);
		}
	}

	private void DoActivation(StartupOptions options)
	{
		// TODO:
	}

	public async Task WaitForStop()
	{
		try
		{
			CancellationTokenSource cancellationTokenSource = new();
			cancellationTokenSource.CancelAfter(3000);
			await RTKApplication.Current.Host.StopAsync(cancellationTokenSource.Token);
		}
		catch (Exception e)
		{
			Logger.LogError(e, "Error stopping host gracefully, shutdown will be forced.");
		}
	}

	public void Restart(bool postUpdate, bool asAdmin = false, IWin32Window? owner = null)
	{
		if (asAdmin)
		{
			DialogResult result = MessageBox.Show(owner, "It is not recommended to run Raid or related programs as Administrator, as it creates unnecessary risk. Are you sure you want to continue?", "Warning", MessageBoxButtons.YesNoCancel);
			if (result != DialogResult.Yes)
			{
				return;
			}
		}

		List<string> args = new() { "--wait", "30000" };
		if (postUpdate)
			args.Add("--post-update");

		ProcessStartInfo psi = new()
		{
			UseShellExecute = asAdmin,
			FileName = RegistrySettings.ExecutableName,
			Verb = asAdmin ? "runAs" : string.Empty,
			Arguments = string.Join(" ", args)
		};
		_ = Process.Start(psi);
		Exit();
	}

	public void Exit()
	{
		MainWindow?.Close();
	}
}

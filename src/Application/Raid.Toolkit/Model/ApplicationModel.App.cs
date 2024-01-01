
using System.Text.RegularExpressions;
using Raid.Toolkit.UI.WinUI;

namespace Raid.Toolkit.Model;

public partial class ApplicationModel : AsyncObservable
{
	private readonly DependencySynthesizer Dependencies;
	private readonly IServiceProvider ServiceProvider;
	private readonly StartupOptions InitialOptions;
	private readonly Queue<StartupOptions> ActivationRequests = new();

	public IPackageManager PackageManager => Dependencies.GetRequiredService<IPackageManager>();
	public IAppDispatcher Dispatcher => Dependencies.GetRequiredService<IAppDispatcher>();
	public IModelLoader ModelLoader => Dependencies.GetRequiredService<IModelLoader>();

	public ApplicationModel(StartupOptions initialOptions, IServiceProvider serviceProvider)
		: base(serviceProvider.GetRequiredService<IAppDispatcher>())
	{
		ServiceProvider = serviceProvider;
		InitialOptions = initialOptions;
		Dependencies = new(serviceProvider);
	}

	public async void OnLaunched()
	{
		MainWindow = ActivatorUtilities.CreateInstance<MainWindow>(ServiceProvider);

		await ModelLoader.BuildAndLoad(Array.Empty<Regex>(), Path.Join(Path.GetDirectoryName(Environment.ProcessPath), "bin"));

		await Dependencies.GetRequiredService<IPackageWorkerManager>().StartExtensions();

		while (ActivationRequests.TryDequeue(out StartupOptions? options))
			DoActivation(options);
	}

	internal void OnActivated(StartupOptions options)
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


}

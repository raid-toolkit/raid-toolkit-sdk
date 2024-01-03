using System.Threading;
using Microsoft.Extensions.Hosting;
using Raid.Toolkit.UI.WinUI;
using Raid.Toolkit.UI.WinUI.Base;

using WinUIEx;

namespace Raid.Toolkit
{
	public class AppWinUI : IAppUI, IHostedService, IDisposable
	{
		private MainWindow? MainWindow;
		private readonly IServiceProvider ServiceProvider;
		private readonly IAppDispatcher AppDispatcher;

		private MainWindow MainWindowUnsafe => MainWindow ??= ActivatorUtilities.CreateInstance<MainWindow>(ServiceProvider);
		private Dictionary<Type, RTKWindow> WindowSingletons = new();

		private bool IsDisposed;
		private AppTray? AppTray;

		public SynchronizationContext? SynchronizationContext { get; }

		public AppWinUI(IServiceProvider serviceProvider, IAppDispatcher appDispatcher)
		{
			ServiceProvider = serviceProvider;
			AppDispatcher = appDispatcher;
			SynchronizationContext = SynchronizationContext.Current;
		}

		public void ShowMain()
		{
			AppDispatcher.Dispatch(() =>
			{
				MainWindowUnsafe.Activate();
				MainWindowUnsafe.BringToFront();
			});
		}

		private T EnsureWindow<T>(params object[] args) where T : RTKWindow
		{
			if (WindowSingletons.TryGetValue(typeof(T), out RTKWindow? window))
			{
				if (window.Visible)
				{
					return (T)window;
				}

				// cleanup old window
				window.Close();
				WindowSingletons.Remove(typeof(T));
			}

			T newWindow = ActivatorUtilities.CreateInstance<T>(ServiceProvider, args);
			WindowSingletons.Add(typeof(T), newWindow);
			newWindow.Closed += Window_Closed;
			return newWindow;
		}

		private void Window_Closed(object sender, Microsoft.UI.Xaml.WindowEventArgs args)
		{
			if (sender is not RTKWindow)
				return;
			WindowSingletons.Remove(sender.GetType());
		}

		public void ShowExtensionManager()
		{
			AppDispatcher.Dispatch(() =>
			{
				ExtensionsWindow extensionWindow = EnsureWindow<ExtensionsWindow>();
				extensionWindow.Show();
			});
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				AppDispatcher.Dispatch(() =>
				{
					if (disposing)
					{
						AppTray?.Dispose();
						MainWindow?.Close();
						foreach (RTKWindow window in WindowSingletons.Values)
						{
							window.Close();
						}
						System.Windows.Forms.Application.Exit();
					}

					MainWindow = null;
				});

				IsDisposed = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public void ShowSettings()
		{
			AppDispatcher.Dispatch(() =>
			{
				MainWindowUnsafe.OpenSettings();
				ShowMain();
			});
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await AppDispatcher.Post(() =>
			{
				AppTray = ActivatorUtilities.CreateInstance<AppTray>(ServiceProvider);
			});
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public void Run()
		{
			MainWindow ??= ActivatorUtilities.CreateInstance<MainWindow>(ServiceProvider);
		}
	}
}

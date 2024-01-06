using Microsoft.Extensions.Logging;

namespace Raid.Toolkit.Extensibility.Host
{
	public class ClientMenuManager : MenuManager
	{
		private readonly DependencySynthesizer Dependencies;
		private readonly ILogger Logger;

		public ClientMenuManager(IServiceProvider serviceProvider, ILogger<IMenuManager> logger)
		{
			Dependencies = new(serviceProvider);
			Logger = logger;
		}

		public override void AddEntry(IMenuEntry entry)
		{
			Logger.LogWarning("IMenuManager APIs are deprecated. Menu entries should be added using the extension manifest `contributions` property.");
			base.AddEntry(entry);
			try
			{
				IManagedPackage package = Dependencies.GetRequiredService<IManagedPackage>();
				var manifest = package.Bundle.Manifest;
				var contributions = manifest.Contributes ??= new();
				var menuItems = contributions.MenuItems ??= new();
				if (!menuItems.Any(contribution => contribution is MenuContribution menuEntry && menuEntry.DisplayName == entry.DisplayName))
				{
					menuItems.Add(new MenuContribution($"__generated__menuEntry{menuItems.Count}", entry.DisplayName));
					var hostChannel = Dependencies.GetRequiredService<IExtensionHostChannel>();
					package.Bundle.WriteManifest(manifest);
					hostChannel.ManifestLoaded += (sender, e) =>
					{
						throw new Exception();
					};
					hostChannel.ReloadManifest(package.Bundle.Id);
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Backwards Compatibility: An error occured channelling menu entry back to main process. Menu entry will not be displayed (DisplayName='{displayName}')", entry.DisplayName);
			}
		}
	}
}

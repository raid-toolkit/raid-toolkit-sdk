using Microsoft.Extensions.Logging;

using Raid.Toolkit.Extensibility.Helpers;

using System;
using System.Linq;

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
				var contributions = manifest.Contributions ??= new();
				if (!contributions.Any(contribution => contribution is MenuContribution menuEntry && menuEntry.DisplayName == entry.DisplayName))
				{
					contributions.Add(new MenuContribution($"__generated__menuEntry{contributions.Count}", entry.DisplayName));
					var hostChannel = Dependencies.GetRequiredService<IExtensionHostChannel>();
					package.SaveManifest(manifest);
					hostChannel.ReloadManifest(package.Bundle.Id);
				}
			}catch(Exception ex)
			{
				Logger.LogError(ex, "Backwards Compatibility: An error occured channelling menu entry back to main process. Menu entry will not be displayed (DisplayName='{displayName}')", entry.DisplayName);
			}
		}
	}
}

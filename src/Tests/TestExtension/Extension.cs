using Raid.Toolkit.Extensibility;

using System;

namespace TestExtension;
public class Extension : ExtensionPackage
{
	public override void OnActivate(IExtensionHost host)
	{
		host.RegisterMenuEntry(new MenuEntry() { DisplayName = "Foo" });
	}
}

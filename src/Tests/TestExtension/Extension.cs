using Raid.Toolkit.Extensibility;

using System;
using System.Windows.Forms;

namespace TestExtension;
public class Extension : ExtensionPackage
{
	public override void OnActivate(IExtensionHost host)
	{
		MenuEntry entry = new() { DisplayName = "Foo" };
		entry.Activate += (sender, e) =>
		{
			MessageBox.Show("Hello, world!");
		};
		host.RegisterMenuEntry(entry);
	}
}

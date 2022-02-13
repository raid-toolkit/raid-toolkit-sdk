using System;
using System.IO;

namespace Raid.Toolkit.Extensibility
{
	public class ExtensionHost : IExtensionHost
    {
        public ExtensionHost()
        {
        }

        public void FindExtensions()
        {
            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Raid.Toolkit.Extension.*.dll");
            foreach (string file in files)
            {
                using ExtensionSandbox sandbox = new(file);
                var descriptor = sandbox.QueryDescriptor();
                sandbox.Load();
            }
        }
	}
}

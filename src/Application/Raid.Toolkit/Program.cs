using Raid.Toolkit.Extensibility;
using System;
using System.Windows.Forms;

namespace Raid.Toolkit
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

            ExtensionHost host = new();
            host.FindExtensions();

            //Application.Run(new Form1());
		}
	}
}

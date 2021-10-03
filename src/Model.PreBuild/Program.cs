using Raid.Model;
using System;
using System.IO;
using System.Reflection;

[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]

namespace Model.PreBuild
{
	class Program
	{
		static void Main(string[] args)
		{
			using ModelAssemblyResolver resolver = new();

			Assembly asm = AppDomain.CurrentDomain.Load("Raid.Interop");
			string targetFile = Path.Join(args[0], Path.GetFileName(asm.Location));
			Console.WriteLine($"Copying [{asm.Location}] to [{targetFile}]");
			Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
			File.Delete(targetFile);
			File.Copy(asm.Location, targetFile);
		}
	}
}

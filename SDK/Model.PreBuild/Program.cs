using System;
using System.IO;
using System.Reflection;
using Raid.Model;

[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]

namespace Model.PreBuild
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (Environment.GetEnvironmentVariable("IS_CI") == "true")
            {
                return;
            }
            using ModelAssemblyResolver resolver = new(force: Environment.GetEnvironmentVariable("FORCE") == "true");

            Assembly asm = AppDomain.CurrentDomain.Load("Raid.Interop");
            string targetFile = Path.Join(args[0], Path.GetFileName(asm.Location));
            Console.WriteLine($"Copying [{asm.Location}] to [{targetFile}]");
            _ = Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
            File.Delete(targetFile);
            File.Copy(asm.Location, targetFile);
        }
    }
}

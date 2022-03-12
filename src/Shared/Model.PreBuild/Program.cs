using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Raid.Model;

#if NET5_0_OR_GREATER
[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif

namespace Model.PreBuild
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            if (Environment.GetEnvironmentVariable("IS_CI") == "true")
            {
                return;
            }
            using ModelAssemblyResolver resolver = new();
            await resolver.Load(force: Environment.GetEnvironmentVariable("FORCE") == "true");

            Assembly asm = AppDomain.CurrentDomain.Load("Raid.Interop");
            string targetFile = Path.Combine(args[0], Path.GetFileName(asm.Location));
            Console.WriteLine($"Copying [{asm.Location}] to [{targetFile}]");
            _ = Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
            File.Delete(targetFile);
            File.Copy(asm.Location, targetFile);
        }
    }
}

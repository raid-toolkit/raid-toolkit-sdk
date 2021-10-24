using System.Threading.Tasks;
using CommandLine;
using Microsoft.Win32;

namespace Raid.Service
{
    [Verb("register", HelpText = "Registers the service with your system")]
    public class RegisterOptions
    {
        [Option('s', "--startup", HelpText = "Registers the service to start when windows starts")]
        public bool RunOnStartup { get; set; }
    }
    static class RegisterAction
    {
        private const string StartupName = "RaidToolkitService";
        private const string Protocol = "rtk";

        public static Task<int> Execute(RegisterOptions options)
        {
            RegisterStartup(options.RunOnStartup);
            RegisterProtocol(true);
            return Task.FromResult(0);
        }

        public static void RegisterStartup(bool runOnStartup)
        {
            RegistryKey runKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            runKey.DeleteValue(StartupName, false);
            if (runOnStartup)
            {
                runKey.SetValue(StartupName, AppConfiguration.ExecutablePath);
            }
        }

        public static void RegisterProtocol(bool registerProtocolHandler)
        {
            RegistryKey classesKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes", true);
            classesKey.DeleteSubKeyTree(Protocol, false);
            if (registerProtocolHandler)
            {
                RegistryKey classKey = classesKey.CreateSubKey(Protocol);
                classKey.SetValue(null, "URL:Raid Toolkit");
                classKey.SetValue("URL Protocol", "");
                var cmdKey = classKey.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");
                cmdKey.SetValue(null, $"\"{AppConfiguration.ExecutablePath}\" open \"%1\"");
            }
        }

    }
}
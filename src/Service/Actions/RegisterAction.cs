using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Raid.Service
{
    static class RegisterAction
    {
        private const string StartupName = "RaidToolkitService";
        private const string Protocol = "rtk";

        public static int Execute(RegisterOptions options)
        {
            RegisterStartup(options.RunOnStartup);
            RegisterProtocol(options.RegisterProtocolHandler);
            return 0;
        }

        private static void RegisterStartup(bool runOnStartup)
        {
            RegistryKey runKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            runKey.DeleteValue(StartupName, false);
            if (runOnStartup)
            {
                runKey.SetValue(StartupName, Assembly.GetExecutingAssembly().Location);
            }
        }

        private static void RegisterProtocol(bool registerProtocolHandler)
        {
            RegistryKey classesKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes", true);
            classesKey.DeleteSubKeyTree(Protocol, false);
            if (registerProtocolHandler)
            {
                RegistryKey classKey = classesKey.CreateSubKey(Protocol);
                classKey.SetValue(null, "URL:Raid Toolkit");
                classKey.SetValue("URL Protocol", "");
                var cmdKey = classKey.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");
                cmdKey.SetValue(null, $"\"{Assembly.GetExecutingAssembly().Location}\" \"%1\"");
            }
        }

    }
}
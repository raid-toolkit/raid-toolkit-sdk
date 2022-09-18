using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Raid.Toolkit.App;
using Raid.Toolkit.App.Tasks;
using Raid.Toolkit.App.Tasks.Base;
using Raid.Toolkit.Preamble;

namespace Raid.Toolkit
{
    /// <summary>
    /// Program class
    /// </summary>
    public static class Program
    {
        [DllImport("Microsoft.ui.xaml.dll")]
        private static extern void XamlCheckProcessRequirements();

        [STAThread]
        static async Task<int> Main(string[] args)
        {
            CommonOptions.Parse(args);
            AppHost.EnableLogging = CommonOptions.Value?.DisableLogging ?? true;

            Entrypoint entry = new();
            CommandTaskManager commandManager = entry.CreateInstance<CommandTaskManager>();
            ICommandTask? task = commandManager.Parse(args);
            if (task == null)
                return 255;

            return await task.Invoke();
        }
    }
}

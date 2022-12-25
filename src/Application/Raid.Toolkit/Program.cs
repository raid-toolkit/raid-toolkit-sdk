using System;

using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Application.Core;
using System.Threading.Tasks;
using Raid.Toolkit.Application.Core.Commands;
using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.UI.Forms;
using Raid.Toolkit.UI.WinUI;
using GitHub;
using System.Linq;
using Raid.Toolkit.Common;
using System.Diagnostics;

namespace Raid.Toolkit
{
    internal static class Program
    {
        private static string CleanArgument(string arg) => arg switch
        {
            "-Embedding" => "--Embedding",
            "----AppNotificationActivated:" => "----AppNotificationActivated",
            _ => arg,
        };
        [STAThread]
        private static async Task<int> Main(string[] args)
        {
            if (RegistrySettings.DebugStartup && !Debugger.IsAttached)
            {
                Debugger.Launch();
            }
            args = args.Select(CleanArgument).ToArray();

            CommonOptions.Parse(args);
            AppHost.EnableLogging = !CommonOptions.Value.DisableLogging;
            AppHost.ForceRebuild = CommonOptions.Value.ForceRebuild;

            IEntrypoint entrypoint = CommonOptions.Value.RenderEngine switch
            {
                RenderingEngine.WinUI => new Entrypoint<AppWinUI, WinUIProgramHost>(),
                _ => new Entrypoint<AppWinUI, WinUIProgramHost>(),
            };
            CommandTaskManager commandManager = entrypoint.CreateInstance<CommandTaskManager>();
            ICommandTask? task = commandManager.Parse(args);
            if (task == null)
                return 255;

            return await task.Invoke();
        }
    }
}

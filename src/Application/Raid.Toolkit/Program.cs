using System;

using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Application.Core;
using System.Threading.Tasks;
using Raid.Toolkit.Application.Core.Commands;
using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.UI.Forms;
using Raid.Toolkit.UI.WinUI;

namespace Raid.Toolkit
{
    internal static class Program
    {
        [STAThread]
        private static async Task<int> Main(string[] args)
        {
            CommonOptions.Parse(args);
            AppHost.EnableLogging = !CommonOptions.Value.DisableLogging;
            AppHost.ForceRebuild = CommonOptions.Value.ForceRebuild;

            IEntrypoint entrypoint = CommonOptions.Value.RenderEngine switch
            {
                RenderingEngine.WinForms => new Entrypoint<AppForms, FormsProgramHost>(),
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

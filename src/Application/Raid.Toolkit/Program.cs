using System;

using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Application.Core;
using System.Threading.Tasks;
using Raid.Toolkit.Application.Core.Commands;
using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.UI.Forms;

namespace Raid.Toolkit
{
    internal static class Program
    {
        [STAThread]
        private static async Task<int> Main(string[] args)
        {
            CommonOptions.Parse(args);
            AppHost.EnableLogging = CommonOptions.Value?.DisableLogging ?? true;

            Entrypoint<AppForms, FormsProgramHost> entry = new();
            CommandTaskManager commandManager = entry.CreateInstance<CommandTaskManager>();
            ICommandTask? task = commandManager.Parse(args);
            if (task == null)
                return 255;

            return await task.Invoke();
        }
    }
}

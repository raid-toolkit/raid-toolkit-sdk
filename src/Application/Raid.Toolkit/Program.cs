using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Application.Core;
using SuperSocket.Command;

using FormsApplication = System.Windows.Forms.Application;
using System.Threading.Tasks;
using Raid.Toolkit.Application.Core.Tasks;
using Raid.Toolkit.Application.Core.Tasks.Base;
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

            Entrypoint<AppForms, ProgramHost> entry = new();
            CommandTaskManager commandManager = entry.CreateInstance<CommandTaskManager>();
            ICommandTask? task = commandManager.Parse(args);
            if (task == null)
                return 255;

            return await task.Invoke();
        }
    }
}

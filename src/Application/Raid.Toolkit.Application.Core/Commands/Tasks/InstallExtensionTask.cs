using Raid.Toolkit.Application.Core.Commands.Matchers;
using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.DataModel;

namespace Raid.Toolkit.Application.Core.Commands.Tasks
{
    internal class InstallExtensionTask : ICommandTask
    {
        private readonly InstallExtensionOptions Options;

        public InstallExtensionTask(InstallExtensionOptions options)
        {
            Options = options;
        }

        public async Task<int> Invoke()
        {
            await AppHost.EnsureProcess();

            RaidToolkitClientBase client = new();
            client.Connect();
            return await client.MakeApi<ActivationApi>().Activate(new Uri("rtk://install-extension"), new string[] { Options.PackagePath });
        }
    }
}

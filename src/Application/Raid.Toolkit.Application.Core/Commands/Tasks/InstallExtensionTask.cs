using Raid.Toolkit.Application.Core.Commands.Matchers;
using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Application.Core.Commands.Base;

namespace Raid.Toolkit.Application.Core.Commands.Tasks
{
    internal class InstallExtensionTask : ICommandTask
    {
        private readonly InstallExtensionOptions Options;

        public InstallExtensionTask(InstallExtensionOptions options)
        {
            Options = options;
        }

        public Task<int> Invoke()
        {
            return AppHost.Activate(new Uri("rtk://install-extension"), Options.PackagePath);
        }
    }
}

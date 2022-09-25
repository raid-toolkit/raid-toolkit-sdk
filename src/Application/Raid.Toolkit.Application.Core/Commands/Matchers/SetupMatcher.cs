using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Application.Core.Commands.Tasks;
using Raid.Toolkit.Application.Core.Commands.Base;

using Raid.Toolkit.Common;
using Raid.Toolkit.Application.Core.Host;

namespace Raid.Toolkit.Application.Core.Commands.Matchers
{
    internal class SetupMatcher : CommandTaskMatcher<RunOptions>
    {
        public static bool ShouldRunSetup(RunOptions options)
        {
            if (options.Standalone || options.Debug)
                return false;

            if (!RegistrySettings.IsInstalled)
                return true;

            if (AppHost.ExecutablePath.ToLowerInvariant() != RegistrySettings.InstalledExecutablePath.ToLowerInvariant())
                return true;

            return false;
        }
        public SetupMatcher(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public override ICommandTask? Match(RunOptions options)
        {
            if (!ShouldRunSetup(options))
                return null;
            return ActivatorUtilities.CreateInstance<SetupTask>(ServiceProvider);
        }
    }
}

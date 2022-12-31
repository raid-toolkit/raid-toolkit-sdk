using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Application.Core.Commands.Tasks;

namespace Raid.Toolkit.Application.Core.Commands.Matchers
{
    [Verb("install", HelpText = "Installs an extension")]
    public class InstallExtensionOptions : CommonOptions
    {
        [Value(0, MetaName = "rtkx", HelpText = "Path to RTKX package to install")]
        public string PackagePath { get; set; } = string.Empty;

        [Option('a', "accept", Hidden = true)]
        public bool Accept { get; set; } = false;
    }

    internal class InstallExtensionMatcher : CommandTaskMatcher<InstallExtensionOptions>
    {
        public InstallExtensionMatcher(IServiceProvider serviceProvider) : base(serviceProvider)
        { }
        public override ICommandTask? Match(InstallExtensionOptions options)
        {
            return ActivatorUtilities.CreateInstance<InstallExtensionTask>(ServiceProvider, options);
        }
    }
}

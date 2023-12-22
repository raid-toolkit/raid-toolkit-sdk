using CommandLine;

using Microsoft.Extensions.DependencyInjection;

using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Application.Core.Commands.Tasks;

namespace Raid.Toolkit.Application.Core.Commands.Matchers
{
    [Verb("run", isDefault: true, HelpText = "Runs the service")]
    public class RunOptions : CommonOptions
    {
        [Value(0, MetaName = "rtkx", HelpText = "Path to rtkx package to install", Hidden = true)]
        public string PackagePath { get; set; } = string.Empty;

        [Option('s', "standalone", HelpText = "Runs in standalone mode")]
        public bool Standalone { get; set; }

        [Option('n', "no-ui", HelpText = "Runs without UI (only valid for standalone mode)")]
        public bool NoUI { get; set; }

        [Option('w', "wait", HelpText = "Wait <ms> for an existing instance to shut down before starting")]
        public int? Wait { get; set; }

        [Option('u', "post-update")]
        public bool Update { get; set; }

        [Option('p', "debug-package", Hidden = true)]
        public string? DebugPackage { get; set; }

        [Option('z', "no-webservice", Hidden = true)]
        public bool NoWebService { get; set; }

        [Option(longName: "--AppNotificationActivated")]
        public bool AppNotificationActivated { get; set; }
    }

    internal class DefaultMatcher : CommandTaskMatcher<RunOptions>
    {
        public DefaultMatcher(IServiceProvider serviceProvider) : base(serviceProvider)
        { }
        public override ICommandTask? Match(RunOptions options)
        {
            if (!string.IsNullOrEmpty(options.PackagePath))
            {
                return ActivatorUtilities.CreateInstance<InstallExtensionTask>(ServiceProvider, new InstallExtensionOptions() { PackagePath = options.PackagePath });
            }
            return ActivatorUtilities.CreateInstance<RunTask>(ServiceProvider, options);
        }
    }
}

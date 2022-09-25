using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine.Text;
using CommandLine;
using Raid.Toolkit.Application.Core.Commands;
using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Application.Core.Commands.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Raid.Toolkit.Application.Core.Commands.Matchers
{
    [Verb("install", HelpText = "Installs an extension")]
    public class InstallExtensionOptions : CommonOptions
    {
        [Value(0, MetaName = "rtkx", HelpText = "Path to RTKX package to install")]
        public string? PackagePath { get; set; }

        [Option('y', "accept")]
        public bool Accept { get; set; }

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

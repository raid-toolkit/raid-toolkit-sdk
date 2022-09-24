using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Application.Core.Commands.Tasks;
using Raid.Toolkit.Application.Core.Commands.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine.Text;
using CommandLine;

namespace Raid.Toolkit.Application.Core.Commands.Matchers
{
    [Verb("install", HelpText = "Installs an extension")]
    public class InstallOptions : CommonOptions
    {
        [Value(0, MetaName = "rtkx", HelpText = "Path to RTKX package to install")]
        public string? PackagePath { get; set; }

        [Option('y', "accept")]
        public bool Accept { get; set; }

    }

    internal class InstallMatcher : CommandTaskMatcher<InstallOptions>
    {
        public InstallMatcher(IServiceProvider serviceProvider) : base(serviceProvider)
        { }
        public override ICommandTask? Match(InstallOptions options)
        {
            return ActivatorUtilities.CreateInstance<InstallExtensionTask>(ServiceProvider, options);
        }
    }
}

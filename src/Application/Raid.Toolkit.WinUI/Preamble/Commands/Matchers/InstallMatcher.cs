using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine.Text;
using CommandLine;
using Raid.Toolkit.App.Tasks.Base;
using Raid.Toolkit.Preamble.Commands.Tasks;
using Raid.Toolkit.App.Tasks;

namespace Raid.Toolkit.Preamble.Commands.Matchers
{
    [Verb("install", HelpText = "Installs an extension")]
    internal class InstallOptions : CommonOptions
    {
        [Value(0, MetaName = "rtkx", HelpText = "Path to RTKX package to install")]
        public string? PackagePath { get; set; }

        [Option('y', "accept")]
        public bool Accept { get; set; }

    }
    internal class InstallMatcher : CommandTaskMatcher<InstallOptions>
    {
        public override ICommandTask? Parse(InstallOptions options)
        {
            return new InstallTask(options);
        }
    }
}

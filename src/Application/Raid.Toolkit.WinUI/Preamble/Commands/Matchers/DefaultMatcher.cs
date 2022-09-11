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

namespace Raid.Toolkit.Preamble.Tasks.Matchers
{
    [Verb("run", isDefault: true, HelpText = "Runs the service")]
    internal class RunOptions : CommonOptions
    {
        [Option('s', "standalone", HelpText = "Runs in standalone mode")]
        public bool Standalone { get; set; }

        [Option('n', "no-ui", HelpText = "Runs without UI (only valid for standalone mode)")]
        public bool NoUI { get; set; }

        [Option('w', "wait", HelpText = "Wait <ms> for an existing instance to shut down before starting")]
        public int? Wait { get; set; }

        [Option('u', "post-update")]
        public bool Update { get; set; }

        [Option('d', "debug", Hidden = true)]
        public bool Debug { get; set; }

        [Option('p', "debug-package", Hidden = true)]
        public string? DebugPackage { get; set; }

        [Option('m', "no-default-packages", Hidden = true)]
        public bool NoDefaultPackages { get; set; }

        [Option('i', "--interop-dir", Hidden = true)]
        public string? InteropDirectory { get; set; }
    }

    internal class DefaultMatcher : CommandTaskMatcher<RunOptions>
    {
        public override ICommandTask? Parse(RunOptions options)
        {
            return new RunTask(options);
        }
    }
}

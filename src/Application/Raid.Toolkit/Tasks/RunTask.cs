using System.Threading.Tasks;
using System.Windows.Forms;
using CommandLine;

namespace Raid.Toolkit
{
    [Verb("run", isDefault: true, HelpText = "Runs the service")]
    internal class RunOptions
    {
        [Option('s', "standalone", HelpText = "Runs in standalone mode")]
        public bool Standalone { get; set; }

        [Option('n', "no-ui", HelpText = "Runs without UI (only valid for standalone mode)")]
        public bool NoUI { get; set; }

        [Option('w', "wait", HelpText = "Wait <ms> for an existing instance to shut down before starting")]
        public int? Wait { get; set; }

        [Option('u', "post-update")]
        public bool Update { get; set; }
    }

    internal class RunTask : CommandTaskBase<RunOptions>
    {
        public RunTask() { }

        protected override Task<int> Invoke(RunOptions options)
        {
            Application.Run(new Form1());
            return Task.FromResult(0);
        }
    }
}
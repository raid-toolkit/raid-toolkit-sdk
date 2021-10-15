using CommandLine;

namespace Raid.Service
{
    [Verb("register", HelpText = "Registers the service with your system")]
    public class RegisterOptions
    {
        [Option('s', "--startup", HelpText = "Registers the service to start when windows starts")]
        public bool RunOnStartup { get; set; }

        [Option('r', "--register-protocol-handler", HelpText = "Registers rtk:// protocol handler")]
        public bool RegisterProtocolHandler { get; set; }
    }

    [Verb("run", isDefault: true, HelpText = "Runs the service")]
    public class RunOptions
    {
        [Option('s', "--standalone", HelpText = "Runs in standalone mode")]
        public bool Standalone { get; set; }

        [Option('n', "--no-ui", HelpText = "Runs without UI (only valid for standalone mode)")]
        public bool NoUI { get; set; }
    }
}
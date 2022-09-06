using System;
using System.Diagnostics;
using CommandLine;
using Raid.Toolkit.App.Tasks.Base;
using Raid.Toolkit.Common;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility.Host;

namespace Raid.Toolkit.App.Tasks
{
    [Verb("activate", HelpText = "Activates an extension or function of Raid Toolkit")]
    internal class ProtocolActivationOptions
    {
        [Value(0, MetaName = "uri", HelpText = "Activation uri")]
        public string? Uri { get; set; }
    }

    internal class ProtocolActivationTask : CommandTaskBase<ProtocolActivationOptions>
    {
        private const int ActivationTimeoutMs = 30000;
        private ProtocolActivationOptions? Options;

        public ProtocolActivationTask()
        {
        }

        public override int Invoke()
        {
            if (Options == null)
                throw new NullReferenceException();

            if (!SingletonProcess.IsRunning)
            {
                ProcessStartInfo psi = new()
                {
                    FileName = AppHost.ExecutableName
                };
                _ = Process.Start(psi);
            }
            DateTime timeout = DateTime.UtcNow.AddMilliseconds(ActivationTimeoutMs);
            while (DateTime.UtcNow < timeout)
            {
                try
                {
                    RaidToolkitClientBase client = new();
                    client.Connect();
                    _ = client.MakeApi<ActivationApi>().Activate(new Uri(Options.Uri!));
                    return 0;
                }
                catch { }
            }

            return 218;
        }

        public override ApplicationStartupCondition Parse(ProtocolActivationOptions options)
        {
            Options = options;
            ApplicationHost.Enabled = false;
            return ApplicationStartupCondition.None;
        }
    }
}

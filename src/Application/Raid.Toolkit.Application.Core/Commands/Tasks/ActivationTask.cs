using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Application.Core.Commands.Matchers;
using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.DataModel;

namespace Raid.Toolkit.Application.Core.Commands.Tasks
{
    internal class ActivationTask : ICommandTask
    {
        private readonly ActivationOptions Options;

        public ActivationTask(ActivationOptions options)
        {
            Options = options;
        }

        public async Task<int> Invoke()
        {
            await AppHost.EnsureProcess();

            RaidToolkitClientBase client = new();
            client.Connect();
            return await client.MakeApi<ActivationApi>().Activate(new Uri(Options.Uri), (Options.Arguments ?? Array.Empty<string>()).ToArray());
        }
    }
}

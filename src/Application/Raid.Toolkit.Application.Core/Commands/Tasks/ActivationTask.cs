using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Application.Core.Commands.Matchers;
using Raid.Toolkit.Application.Core.Host;

namespace Raid.Toolkit.Application.Core.Commands.Tasks
{
    internal class ActivationTask : ICommandTask
    {
        private readonly ActivationOptions Options;

        public ActivationTask(ActivationOptions options)
        {
            Options = options;
        }

        public Task<int> Invoke()
        {
            return AppHost.Activate(new Uri(Options.Uri), (Options.Arguments ?? Array.Empty<string>()).ToArray());
        }
    }
}

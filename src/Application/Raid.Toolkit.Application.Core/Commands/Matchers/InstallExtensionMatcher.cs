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
    internal class InstallExtensionMatcher : CommandTaskMatcher<RunOptions>
    {
        public InstallExtensionMatcher(IServiceProvider serviceProvider) : base(serviceProvider)
        { }
        public override ICommandTask? Match(RunOptions options)
        {
            return ActivatorUtilities.CreateInstance<InstallExtensionTask>(ServiceProvider, options);
        }
    }
}

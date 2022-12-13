using CommandLine.Text;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Application.Core.Commands.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.Application.Core.Commands.Matchers
{
    [Verb("activate", HelpText = "Handle protocol activation")]
    public class ActivationOptions: CommonOptions
    {
        [Value(0, MetaName = "uri", HelpText = "Activation uri")]
        public string Uri { get; set; } = string.Empty;

        [Value(1, MetaName = "Arguments", HelpText = "Additional arguments")]
        public IEnumerable<string> Arguments { get; set; } = Array.Empty<string>();
    }

    internal class ActivationMatcher : CommandTaskMatcher<ActivationOptions>
    {
        public ActivationMatcher(IServiceProvider serviceProvider) : base(serviceProvider)
        { }
        public override ICommandTask? Match(ActivationOptions options)
        {
            return ActivatorUtilities.CreateInstance<ActivationTask>(ServiceProvider, options);
        }
    }
}

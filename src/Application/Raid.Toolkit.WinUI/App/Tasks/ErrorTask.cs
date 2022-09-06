using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;
using Raid.Toolkit.App.Tasks.Base;

namespace Raid.Toolkit.App.Tasks
{
    internal class ErrorTask : CommandTaskBase<IEnumerable<Error>>
    {
        private readonly List<Error> Errors = new();

        public override ApplicationStartupCondition Parse(IEnumerable<Error> options)
        {
            Errors.AddRange(options);
            return ApplicationStartupCondition.None;
        }

        public override int Invoke()
        {
            foreach (var error in Errors)
            {
                Console.WriteLine(error.ToString());
            }
            return 1;
        }
    }
}

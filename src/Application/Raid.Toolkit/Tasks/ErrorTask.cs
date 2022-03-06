using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;

namespace Raid.Toolkit
{
    internal class ErrorTask : CommandTaskBase<IEnumerable<Error>>
    {
        protected override Task<int> Invoke(IEnumerable<Error> errors)
        {
            foreach (var error in errors)
            {
                Console.WriteLine(error.ToString());
            }
            return Task.FromResult(1);
        }
    }
}
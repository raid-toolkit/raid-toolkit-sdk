using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine.Text;
using CommandLine;

namespace Raid.Toolkit.App.Tasks
{
    internal class CommonOptions
    {
        public static CommonOptions? Value { get; private set; }

        public static void Parse(string[] arguments)
        {
            Parser parser = new(settings =>
            {
                settings.IgnoreUnknownArguments = true;
            });
            parser.ParseArguments<CommonOptions>(arguments).WithParsed(options => Value = options);
        }

        //[Value(0, MetaName = "Command", HelpText = "Command to run")]
        //public string? PackagePath { get; set; }

        [Option('q', "quiet")]
        public bool DisableLogging { get; set; }
    }
}

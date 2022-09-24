using CommandLine;

namespace Raid.Toolkit.Application.Core.Tasks
{
    public class CommonOptions
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

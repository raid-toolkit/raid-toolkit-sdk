using CommandLine;

namespace Raid.Toolkit.Application.Core.Commands
{
    public enum RenderingEngine
    {
        WinUI,
        WinForms,
    }
    public class CommonOptions
    {
        public static CommonOptions Value { get; private set; } = new();

        public static void Parse(string[] arguments)
        {
            Parser parser = new(settings =>
            {
                settings.IgnoreUnknownArguments = true;
                settings.CaseInsensitiveEnumValues = true;
            });
            parser.ParseArguments<CommonOptions>(arguments).WithParsed(options => Value = options);
        }

        //[Value(0, MetaName = "Command", HelpText = "Command to run")]
        //public string? PackagePath { get; set; }

        [Option('q', "quiet")]
        public bool DisableLogging { get; set; }

        [Option("Embedding")]
        public bool Embedding { get; set; }

        [Option("force-rebuild")]
        public bool ForceRebuild { get; set; }

        [Option("nologo")]
        public bool NoLogo { get; set; }

        [Option('d', "debug", Hidden = true)]
        public bool Debug { get; set; }

        [Option("render-engine")]
        public RenderingEngine RenderEngine { get; set; } = RenderingEngine.WinUI;
    }
}

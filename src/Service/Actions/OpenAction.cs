using CommandLine;

namespace Raid.Service
{
    [Verb("open", HelpText = "Opens a RTK link")]
    public class OpenOptions
    {
    }
    static class OpenAction
    {
        public static int Execute(OpenOptions options)
        {
            return 0;
        }
    }
}
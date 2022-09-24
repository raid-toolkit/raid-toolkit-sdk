using CommandLine;
using Raid.Toolkit.Application.Core.Commands.Base;

namespace Raid.Toolkit.Application.Core.Commands
{
    [Flags]
    internal enum ApplicationStartupCondition
    {
        None = 0,
        Usage = (1 << 0),
        Services = (1 << 1)
    }
    internal static class ParserResultExtensions
    {
        public static object? GetValue<T>(this ParserResult<T> result)
        {
            if (result is NotParsed<object> notParsed)
            {
                return notParsed.Errors;
            }
            if (result is Parsed<object> parsed)
            {
                return parsed.Value;
            }
            return null;
        }
    }
    public class CommandTaskManager
    {
        private readonly List<ICommandTaskMatcher> TaskMatchers;

        public CommandTaskManager(IEnumerable<ICommandTaskMatcher> tasks)
        {
            TaskMatchers = new(tasks);
        }

        public ICommandTask? Parse(string[] args)
        {
            Parser parser = new(settings =>
            {
                settings.IgnoreUnknownArguments = true;
            });
            ParserResult<object> result = parser.ParseArguments(args, TaskMatchers.Select(task => task.OptionsType).ToArray());

            object? valueType = result.GetValue();

            if (valueType == null)
                return null;

            foreach(ICommandTaskMatcher matcher in TaskMatchers)
            {
                if (!valueType.GetType().IsAssignableTo(matcher.OptionsType))
                    continue;
                ICommandTask? task = matcher.Match(valueType);
                if (task == null)
                    continue;
                return task;
            }
            return null;
        }
    }
}

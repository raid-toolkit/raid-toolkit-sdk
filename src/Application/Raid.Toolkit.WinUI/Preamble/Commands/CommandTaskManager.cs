using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CommandLine;
using Raid.Toolkit.App.Tasks.Base;

namespace Raid.Toolkit.App.Tasks
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
    internal class CommandTaskManager
    {
        private readonly List<ICommandTaskMatcher> TaskMatchers;
        private ICommandTaskMatcher? SelectedTaskMatcher;
        private ICommandTask? SelectedTask;

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

            SelectedTaskMatcher = TaskMatchers.FirstOrDefault(task => valueType.GetType().IsAssignableTo(task.OptionsType));
            if (SelectedTaskMatcher == null)
                return null;

            return SelectedTaskMatcher.Parse(valueType);
        }
    }
}

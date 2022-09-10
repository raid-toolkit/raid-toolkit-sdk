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
        private readonly List<ICommandTask> Tasks;
        private ICommandTask? SelectedTask;

        public CommandTaskManager(IEnumerable<ICommandTask> tasks)
        {
            Tasks = new(tasks);
        }

        public ApplicationStartupCondition Parse(string[] args)
        {
            var parser = new Parser(settings =>
            {
                settings.IgnoreUnknownArguments = false;
            });
            ParserResult<object> result = parser.ParseArguments(args, Tasks.Select(task => task.OptionsType).ToArray());

            object? valueType = result.GetValue();

            if (valueType == null)
                return ApplicationStartupCondition.Usage;

            SelectedTask = Tasks.FirstOrDefault(task => valueType.GetType().IsAssignableTo(task.OptionsType));
            if (SelectedTask != null)
            {
                return SelectedTask.Parse(valueType);
            }

            return ApplicationStartupCondition.Usage;
        }

        public Task<int> Execute()
        {
            if (SelectedTask == null)
                return Task.FromResult(255);

            return SelectedTask.Invoke();
        }
    }
}

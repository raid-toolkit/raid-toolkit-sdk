using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;

namespace Raid.Toolkit
{
    [Flags]
    internal enum ApplicationStartupCondition
    {
        None = 0,
        Usage = (1 << 0),
        Services = (1 << 1)
    }
    internal class ApplicationStartupTask
    {
        private readonly List<ICommandTask> Tasks;
        private ICommandTask? SelectedTask;

        public ApplicationStartupTask(IEnumerable<ICommandTask> tasks)
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
            object? valueType = null;
            if (result is NotParsed<object> notParsed)
            {
                valueType = notParsed.Errors;
            }
            if (result is Parsed<object> parsed)
            {
                valueType = parsed.Value;
            }

            if (valueType == null)
                return ApplicationStartupCondition.Usage;

            foreach (var task in Tasks)
            {
                if (valueType.GetType().IsAssignableTo(task.OptionsType))
                {
                    SelectedTask = task;
                    return task.Parse(valueType);
                }
            }

            return ApplicationStartupCondition.Usage;
        }

        public int Execute()
        {
            if (SelectedTask == null)
                return 255;

            return SelectedTask.Invoke();
        }
    }
}

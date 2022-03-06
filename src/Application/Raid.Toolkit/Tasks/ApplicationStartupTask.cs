using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;

namespace Raid.Toolkit
{
    internal class ApplicationStartupTask
    {
        private readonly List<ICommandTask> Tasks;
        public ApplicationStartupTask(IEnumerable<ICommandTask> tasks)
        {
            Tasks = new(tasks);
        }

        public Task<int> Execute(string[] args)
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
                return UsageError();

            foreach (var task in Tasks)
            {
                if (valueType.GetType().IsAssignableTo(task.OptionsType))
                {
                    return task.Invoke(valueType);
                }
            }

            return UsageError();
        }

        private Task<int> UsageError()
        {
            return Task.FromResult(255);
        }
    }
}
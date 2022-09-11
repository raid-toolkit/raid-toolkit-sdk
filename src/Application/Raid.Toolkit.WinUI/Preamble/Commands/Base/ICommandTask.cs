using System;
using System.Threading.Tasks;

namespace Raid.Toolkit.App.Tasks.Base
{
    internal interface ICommandTask
    {
        Task<int> Invoke();
    }

    internal interface ICommandTaskMatcher
    {
        Type OptionsType { get; }
        ICommandTask? Parse(object options);
    }

    internal abstract class CommandTaskMatcher<T> : ICommandTaskMatcher
    {
        public Type OptionsType => typeof(T);

        public abstract ICommandTask? Parse(T options);

        public ICommandTask? Parse(object options)
        {
            return Parse((T)options);
        }
    }
}

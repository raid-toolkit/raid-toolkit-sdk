using System;
using System.Threading.Tasks;

namespace Raid.Toolkit
{
    internal interface ICommandTask
    {
        Type OptionsType { get; }
        ApplicationStartupCondition Parse(object options);
        Task<int> Invoke();
    }

    internal abstract class CommandTaskBase<T> : ICommandTask
    {
        public Type OptionsType => typeof(T);

        public abstract ApplicationStartupCondition Parse(T options);
        public abstract Task<int> Invoke();

        public ApplicationStartupCondition Parse(object options)
        {
            return Parse((T)options);
        }

    }
}

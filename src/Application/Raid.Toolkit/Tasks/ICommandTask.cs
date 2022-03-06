using System;
using System.Threading.Tasks;

namespace Raid.Toolkit
{
    internal interface ICommandTask
    {
        Type OptionsType { get; }
        Task<int> Invoke(object options);
    }

    internal abstract class CommandTaskBase<T> : ICommandTask
    {
        public Type OptionsType => typeof(T);

        public Task<int> Invoke(object options)
        {
            return Invoke((T)options);
        }

        protected abstract Task<int> Invoke(T options);
    }
}

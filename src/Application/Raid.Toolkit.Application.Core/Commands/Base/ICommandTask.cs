namespace Raid.Toolkit.Application.Core.Commands.Base
{
    public interface ICommandTask
    {
        Task<int> Invoke();
    }

    public interface ICommandTaskMatcher
    {
        Type OptionsType { get; }
        ICommandTask? Match(object options);
    }

    public abstract class CommandTaskMatcher<T> : ICommandTaskMatcher
    {
        protected IServiceProvider ServiceProvider;

        protected CommandTaskMatcher(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public Type OptionsType => typeof(T);

        public abstract ICommandTask? Match(T options);

        public ICommandTask? Match(object options)
        {
            return Match((T)options);
        }
    }
}

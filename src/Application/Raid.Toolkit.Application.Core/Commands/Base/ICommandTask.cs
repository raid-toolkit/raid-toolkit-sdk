namespace Raid.Toolkit.Application.Core.Tasks.Base
{
    public interface ICommandTask
    {
        Task<int> Invoke();
    }

    public interface ICommandTaskMatcher
    {
        Type OptionsType { get; }
        ICommandTask? Parse(object options);
    }

    public abstract class CommandTaskMatcher<T> : ICommandTaskMatcher
    {
        protected IServiceProvider ServiceProvider;

        protected CommandTaskMatcher(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public Type OptionsType => typeof(T);

        public abstract ICommandTask? Parse(T options);

        public ICommandTask? Parse(object options)
        {
            return Parse((T)options);
        }
    }
}

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Raid.DataServices
{
    public interface IContext<out T> where T : class
    {
        T Value { get; }
    }

    public static class ContextExtensions
    {
        public static IServiceScope AddContext<T>(this IServiceScope scope, T context) where T : class
        {
            scope.ServiceProvider.GetRequiredService<IContextProvider<T>>().Set(context);
            return scope;
        }
    }

    public class ContextManager<T> : IContext<T> where T : class
    {
        public T Value { get; }
        public ContextManager(IContextProvider<T> contextProvider)
        {
            Value = contextProvider.Value;
            if (Value == null)
            {
                throw new InvalidOperationException("Context is not set");
            }
        }
    }

    public interface IContextProvider<T> : IContext<T> where T : class
    {
        void Set(T value);
    }

    public class ContextProviderManager<T> : IContextProvider<T>, IContext<T> where T : class
    {
        public T Value { get; private set; }

        public void Set(T value)
        {
            if (Value != null)
                throw new InvalidOperationException("Context is already set");

            Value = value;
        }
    }
}
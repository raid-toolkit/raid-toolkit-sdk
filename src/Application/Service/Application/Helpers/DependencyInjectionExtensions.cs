using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Raid.Service
{
    public interface IDependencyTypeFactory<T>
    {
        T Create(IServiceProvider serviceProvider);
    }
    public class DependencyTypeFactory<T, U> : IDependencyTypeFactory<T> where U : T
    {
        public T Create(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<U>();
        }
    }
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddTypesAssignableTo<T>(
            this IServiceCollection collection,
            Expression<Func<IServiceCollection, Func<Type, Type, IServiceCollection>>> expression,
            Assembly assembly = null
        )
        {
            Func<Type, Type, IServiceCollection> fn = expression.Compile().Invoke(collection);
            foreach (var type in (assembly ?? Assembly.GetExecutingAssembly()).GetTypesAssignableTo<T>())
            {
                fn(typeof(T), type);
            }
            return collection;
        }

        public static IServiceCollection AddConcreteTypesAssignableTo<T>(
            this IServiceCollection collection,
            Expression<Func<IServiceCollection, Func<Type, Type, IServiceCollection>>> expression,
            Assembly assembly = null
        )
        {
            Func<Type, Type, IServiceCollection> fn = expression.Compile().Invoke(collection);
            foreach (var type in (assembly ?? Assembly.GetExecutingAssembly()).GetTypesAssignableTo<T>())
            {
                fn(type, type);
            }
            return collection;
        }

        public static IServiceCollection AddTypesAssignableToFactories<T>(
            this IServiceCollection collection,
            Expression<Func<IServiceCollection, Func<Type, Type, IServiceCollection>>> expression,
            Assembly assembly = null
        )
        {
            Func<Type, Type, IServiceCollection> fn = expression.Compile().Invoke(collection);
            foreach (var type in (assembly ?? Assembly.GetExecutingAssembly()).GetTypesAssignableTo<T>())
            {
                fn(
                    typeof(IDependencyTypeFactory<>).MakeGenericType(typeof(T)),
                    typeof(DependencyTypeFactory<,>).MakeGenericType(typeof(T), type)
                );
                collection.AddScoped(typeof(T), type);
            }
            return collection;
        }

        public static IServiceCollection AddHostedServiceSingleton<T>(
            this IServiceCollection collection) where T : class, IHostedService
        {
            collection.AddSingleton<T>();
            collection.AddHostedService<T>(provider => provider.GetService<T>());
            return collection;
        }

        public static IServiceCollection AddHostedServiceSingleton<U, T>(
            this IServiceCollection collection)
            where U : class
            where T : class, IHostedService, U
        {
            collection.AddSingleton<U, T>();
            collection.AddHostedService<T>(provider => provider.GetService<U>() as T);
            return collection;
        }
    }
}

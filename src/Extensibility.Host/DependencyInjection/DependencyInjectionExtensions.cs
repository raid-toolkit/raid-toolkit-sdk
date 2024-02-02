using System;

using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class DependencyInjectionExtensions
	{
		public static IServiceCollection AddHostedServiceSingleton<T>(
			this IServiceCollection collection) where T : class, IHostedService
		{
			collection.AddSingleton<T>();
			collection.AddHostedService<T>(provider => provider.GetRequiredService<T>());
			return collection;
		}

		public static IServiceCollection AddHostedServiceSingleton<U, T>(
			this IServiceCollection collection)
			where U : class
			where T : class, IHostedService, U
		{
			collection.AddSingleton<U, T>();
			collection.AddHostedService(
				provider => provider.GetRequiredService<U>() as T
					?? throw new NotSupportedException("Required service is not available"));
			return collection;
		}
	}
}

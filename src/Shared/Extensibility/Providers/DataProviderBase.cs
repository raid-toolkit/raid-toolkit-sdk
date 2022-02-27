using System;
using Il2CppToolkit.Runtime;
using Raid.DataServices;

namespace Raid.Toolkit.Extensibility.Providers
{
	public interface IContextDataProvider
	{
		Type ContextType { get; }
		DataTypeAttribute DataType { get; }

		bool Upgrade(IDataContext context, Version dataVersion);
		bool Update(Il2CsRuntimeContext scope, IDataContext context);
	}
	public interface IContextDataProvider<TContext> : IContextDataProvider
		where TContext : class, IDataContext
	{
		bool Upgrade(TContext context, Version dataVersion);
		bool Update(Il2CsRuntimeContext scope, TContext context);
	}

	public abstract class DataProviderBase<TContext, TData> : IContextDataProvider<TContext>
		where TContext : class, IDataContext
		where TData : class
	{
		public DataTypeAttribute DataType => PrimaryProvider.DataType.Attribute;
		public Type ContextType => typeof(TContext);

		protected readonly IDataResolver<TContext, TData> PrimaryProvider;

		public DataProviderBase(IDataResolver<TContext, TData> primaryProvider)
		{
			PrimaryProvider = primaryProvider;
		}

		public virtual TData GetValue(TContext context)
		{
			return PrimaryProvider.TryRead(context, out TData value) ? value : default;
		}

		public virtual bool Upgrade(TContext context, Version dataVersion)
		{
			return false;
		}

		public abstract bool Update(Il2CsRuntimeContext scope, TContext context);

		bool IContextDataProvider.Upgrade(IDataContext context, Version dataVersion)
		{
			return Upgrade(context as TContext, dataVersion);
		}

		bool IContextDataProvider.Update(Il2CsRuntimeContext scope, IDataContext context)
		{
			return Update(scope, context as TContext);
		}
	}
}

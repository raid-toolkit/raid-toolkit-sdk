using Raid.Toolkit.Extensibility.Providers;
using System;
using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility
{
	public interface IContextDataManager
	{
		public IReadOnlyList<IContextDataProvider> Providers { get; }
		public IDisposable AddProvider<T>() where T : IContextDataProvider;
	}
}

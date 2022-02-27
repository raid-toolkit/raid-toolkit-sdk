using Il2CppToolkit.Runtime;
using Raid.DataServices;
using Raid.Toolkit.Extensibility.Providers;
using System;
using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility
{
	public interface IContextDataManager
	{
		public IDisposable AddProvider<T>() where T : IContextDataProvider;

		public bool Update<T>(Il2CsRuntimeContext runtime, T context) where T : IDataContext;
	}
}

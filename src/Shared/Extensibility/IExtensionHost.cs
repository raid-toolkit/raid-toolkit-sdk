using Raid.Toolkit.Extensibility.Providers;
using Raid.Toolkit.Extensibility.Services;
using System;

namespace Raid.Toolkit.Extensibility
{
    public interface IExtensionHost
    {
        IDisposable RegisterMessageScopeHandler<T>() where T : IMessageScopeHandler;
        IDisposable RegisterDataProvider<T>() where T : IDataProvider;
    }
}

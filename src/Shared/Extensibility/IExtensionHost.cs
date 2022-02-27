using Raid.Toolkit.Extensibility.Providers;
using Raid.Toolkit.Extensibility.Services;
using System;

namespace Raid.Toolkit.Extensibility
{
    public interface IExtensionHost
    {
        IDisposable RegisterMessageScopeHandler(IMessageScopeHandler handler);
        IDisposable RegisterContextDataProvider<T>() where T : IContextDataProvider;
    }
}

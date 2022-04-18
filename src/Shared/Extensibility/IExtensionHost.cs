using Raid.Toolkit.Extensibility.Providers;
using Raid.Toolkit.Extensibility.Services;
using System;

namespace Raid.Toolkit.Extensibility
{
    public interface IExtensionHost
    {
        T CreateInstance<T>() where T : IDisposable;
        T GetInstance<T>() where T : IDisposable;
        IDisposable RegisterMessageScopeHandler<T>() where T : IMessageScopeHandler;
        IDisposable RegisterDataProvider<T>() where T : IDataProvider;
        IDisposable RegisterBackgroundService<T>() where T : IBackgroundService;
        IDisposable RegisterMenuEntry(IMenuEntry entry);
    }
}

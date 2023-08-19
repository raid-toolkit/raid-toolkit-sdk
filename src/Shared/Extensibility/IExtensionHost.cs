using System;
using System.Windows.Forms;
using Raid.Toolkit.Extensibility.Providers;
using Raid.Toolkit.Extensibility.Services;

namespace Raid.Toolkit.Extensibility
{
    public interface IExtensionHost
    {
        T CreateInstance<T>(params object[] args) where T : IDisposable;

        IExtensionStorage GetStorage(bool enableCache);
        IExtensionStorage GetStorage(IAccount account, bool enableCache);

        IDisposable RegisterMessageScopeHandler<T>(T handler) where T : IMessageScopeHandler;
        IDisposable RegisterDataProvider<T>(T provider) where T : IDataProvider;
        IDisposable RegisterBackgroundService<T>(T service) where T : IBackgroundService;
        IDisposable RegisterAccountExtension<T>(T factory) where T : IAccountExtensionFactory;

        IDisposable RegisterMenuEntry(IMenuEntry entry);
        IDisposable RegisterWindow<T>(WindowOptions options) where T : class;
        IWindowAdapter<T> CreateWindow<T>() where T : class;

        bool CanShowUI { get; }

        [Obsolete] IDisposable RegisterMessageScopeHandler<T>() where T : IMessageScopeHandler;
        [Obsolete] IDisposable RegisterDataProvider<T>() where T : IDataProvider;
        [Obsolete] IDisposable RegisterBackgroundService<T>() where T : IBackgroundService;
    }
}

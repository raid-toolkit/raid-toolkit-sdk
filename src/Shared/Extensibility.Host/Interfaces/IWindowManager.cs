using System;
using System.Windows;
using System.Windows.Forms;

namespace Raid.Toolkit.Extensibility.Host
{
    public interface IWindowManager
    {
        void RegisterWindow<T>(WindowOptions options) where T : class, IDisposable;
        void UnregisterWindow<T>() where T : class, IDisposable;
        T CreateWindow<T>() where T : class, IDisposable;

        void RestoreWindows();
        bool CanShowUI { get; internal set; }
    }
}

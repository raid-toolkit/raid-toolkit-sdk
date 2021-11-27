using System;
using System.Reflection;

namespace Raid.Model
{
    public class ModelAssemblyResolver : IDisposable
    {
        private readonly ModelLoader m_loader;
        public static string LoadedVersion { get; private set; }
        public static string CurrentVersion => GameInfo.Version;
        public static Version InteropVersion { get; private set; }
        public static PlariumPlayAdapter.GameInfo GameInfo => ModelLoader.GetGameInfo();

        public ModelAssemblyResolver(bool force = false)
        {
            m_loader = new();
            AppDomain.CurrentDomain.AssemblyResolve += HandleAssemblyResolve;
            _ = m_loader.Load(force);
            LoadedVersion = m_loader.GameVersion;
            InteropVersion = m_loader.InteropVersion;
        }

        private Assembly HandleAssemblyResolve(object source, ResolveEventArgs e)
        {
            return new AssemblyName(e.Name).Name != "Raid.Interop" ? null : m_loader.Load();
        }

        protected virtual void Dispose(bool disposing)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= HandleAssemblyResolve;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
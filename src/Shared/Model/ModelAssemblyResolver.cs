using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Raid.Model
{
    public class ModelAssemblyResolver : IDisposable
    {
        private readonly ModelLoader m_loader;
        public static string LoadedVersion { get; private set; }
        public static string CurrentVersion => GameInfo.Version;
        public static Version InteropVersion { get; private set; }
        public static PlariumPlayAdapter.GameInfo GameInfo => ModelLoader.GetGameInfo();
        public event EventHandler<ModelLoadStateEventArgs> OnStateUpdated;

        public ModelAssemblyResolver()
        {
            m_loader = new();
        }

        public async Task Load(bool force = false)
        {
            // run load on separate thread so that we don't block the main thread
            AppDomain.CurrentDomain.AssemblyResolve += HandleAssemblyResolve;
            _ = await m_loader.Load(state => OnStateUpdated?.Invoke(this, new(state)), force);
            LoadedVersion = m_loader.GameVersion;
            InteropVersion = m_loader.InteropVersion;
        }

        private Assembly HandleAssemblyResolve(object source, ResolveEventArgs e)
        {
            if (new AssemblyName(e.Name).Name != "Raid.Interop")
                return null;
            var loadTask = m_loader.Load();
            loadTask.Wait();
            return loadTask.Result;
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

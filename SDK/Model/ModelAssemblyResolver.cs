using System;
using System.Reflection;

namespace Raid.Model
{
    public class ModelAssemblyResolver : IDisposable
    {
        private readonly ModelLoader m_loader;

        public ModelAssemblyResolver()
        {
            m_loader = new();
            AppDomain.CurrentDomain.AssemblyResolve += HandleAssemblyResolve;
            Assembly.Load("Raid.Interop");
        }

        Assembly HandleAssemblyResolve(object source, ResolveEventArgs e)
        {
            if (new AssemblyName(e.Name).Name != "Raid.Interop")
			{
                return null;
			}
            return m_loader.Load();
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
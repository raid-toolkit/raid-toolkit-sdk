using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
    public interface IModelLoader
    {
        public enum LoadState
        {
            Initialize,
            Load,
            Rebuild,
            Ready,
            Error,
        }

        public class ModelLoaderEventArgs : EventArgs
        {
            public LoadState LoadState { get; set; }
            public ModelLoaderEventArgs(LoadState state) => LoadState = state;
        }

        public event EventHandler<ModelLoaderEventArgs> OnStateUpdated;

        public Task<Assembly> Load(IEnumerable<Regex> regices, bool force = false);
    }
}
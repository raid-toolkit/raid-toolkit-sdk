using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Raid.Toolkit
{
    public interface IModelLoader
    {
        public enum LoadState
        {
            Initialize,
            Rebuild,
            Ready,
            Loaded,
            Error,
        }

        public class ModelLoaderEventArgs : EventArgs
        {
            public LoadState LoadState { get; set; }
            public ModelLoaderEventArgs(LoadState state) => LoadState = state;
        }

        public string OutputDirectory { get; set; }

        public event EventHandler<ModelLoaderEventArgs> OnStateUpdated;

        public Task<Assembly> BuildAndLoad(IEnumerable<Regex> regices, bool force = false);
    }
}

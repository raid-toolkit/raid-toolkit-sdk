using System;

namespace Raid.Toolkit.Extensibility.Host
{
    public enum ModelLoadState
    {
        Initialize,
        Load,
        Rebuild,
        Ready,
        Error,
    }
    public class ModelLoadStateEventArgs : EventArgs
    {
        public ModelLoadState LoadState { get; set;}
        public ModelLoadStateEventArgs(ModelLoadState state) => LoadState = state;
    }
}

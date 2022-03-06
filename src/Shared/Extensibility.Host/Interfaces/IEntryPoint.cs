namespace Raid.Toolkit.Extensibility.Host
{
    public interface IRunArguments
    {
        public bool Standalone { get; }
        public bool NoUI { get; }
        public int? Wait { get; }
        public bool Update { get; }
    }
    public interface IEntryPoint
    {
        public void Run(IRunArguments arguments);
        public void Restart(IRunArguments arguments);
        public void Exit();
    }
}
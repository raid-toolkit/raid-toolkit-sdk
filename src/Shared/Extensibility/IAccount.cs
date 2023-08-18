using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extensibility
{
    public struct AccountInfo
    {
        public string AccountName { get; }
        public string AvatarUrl { get; }
    }
    public interface IAccount
    {
        string Id { get; }
        string AccountName { get; }
        string AvatarUrl { get; }

        bool IsOnline { get; }
        Il2CsRuntimeContext? Runtime { get; }
    }
}

using System.Diagnostics.CodeAnalysis;
using Il2CppToolkit.Runtime;

using Raid.Toolkit.DataModel;

namespace Raid.Toolkit.Extensibility
{
    public interface IAccount
    {
        string Id { get; }
        string AccountName { get; }
        string AvatarUrl { get; }

        AccountBase AccountInfo { get; }

        bool IsOnline { get; }
        Il2CsRuntimeContext? Runtime { get; }

        bool TryGetApi<T>([NotNullWhen(true)] out T? api) where T : class;
    }
}

using System;
using System.Diagnostics.CodeAnalysis;
using Il2CppToolkit.Runtime;

using Raid.Toolkit.DataModel;

namespace Raid.Toolkit.Extensibility
{
    public class AccountEventArgs : EventArgs
    {
        public AccountEventArgs() { }
    }
    public interface IAccount
    {
        event EventHandler<AccountEventArgs> OnConnected;
        event EventHandler<AccountEventArgs> OnDisconnected;

        string Id { get; }
        string AccountName { get; }
        string AvatarUrl { get; }

        AccountBase AccountInfo { get; }

        [MemberNotNullWhen(true, nameof(Runtime))] bool IsOnline { get; }
        Il2CsRuntimeContext? Runtime { get; }

        bool TryGetApi<T>([NotNullWhen(true)] out T? api) where T : class;
    }
}

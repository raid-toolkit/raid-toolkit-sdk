using System.Collections;
using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility.Host;

public interface IAccountManager
{
    IEnumerable<IAccount> Accounts { get; }
    void RegisterAccountExtension<T>(T factory) where T : IAccountExtensionFactory;
    void UnregisterAccountExtension<T>(T factory) where T : IAccountExtensionFactory;
}

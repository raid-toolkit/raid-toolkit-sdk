using Raid.Toolkit.Extensibility;

namespace Raid.Toolkit.Extension.Account;

public class AccountDataExtensionFactory<T> : IAccountExtensionFactory
    where T : AccountDataExtensionBase
{
    private readonly IExtensionHost Host;
    private readonly bool StaticData;
    public AccountDataExtensionFactory(IExtensionHost host, bool staticData)
    {
        Host = host;
        StaticData = staticData;
    }

    public IAccountExtension Create(IAccount account)
    {
        return Host.CreateInstance<T>(account,
            StaticData
            ? Host.GetStorage(true)
            : Host.GetStorage(account, true));
    }
}

namespace Raid.Toolkit.Extensibility.Host;

public interface IAccountManager
{
    void RegisterAccountExtension<T>(T factory) where T : IAccountExtensionFactory;
    void UnregisterAccountExtension<T>(T factory) where T : IAccountExtensionFactory;
}

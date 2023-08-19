using Raid.Toolkit.Extensibility;

namespace Raid.Toolkit.Extension.Account;

public interface GetAccountDataApi<T> where T : class
{
    bool TryGetData(out T data);
}

using System.Collections.Generic;
using System.Linq;
using Raid.Toolkit.Extensibility;

namespace Raid.Toolkit.Extension;

public class AccountDataSpec<T> where T : class
{
	public AccountDataSpec()
	{
	}

	public T Get(IAccount account)
	{
		if (!account.TryGetApi<IGetAccountDataApi<T>>(out var api)
			|| !api.TryGetData(out T data))
			throw new System.NullReferenceException("Could not obtain value");

		return data;
	}
}
public class StaticDataSpec<T> where T : class
{
	public StaticDataSpec()
	{
	}

	public T Get(IExtensionHost host)
	{
		IAccount? account = host.GetAccounts().FirstOrDefault()
			?? throw new System.NullReferenceException("Static data not yet extracted. Start the game and allow extraction of an account to access this data.");

		if (!account.TryGetApi<IGetAccountDataApi<T>>(out var api)
			|| !api.TryGetData(out T data))
			throw new System.NullReferenceException("Could not obtain value");

		return data;
	}

	public T Get(IEnumerable<IAccount> accounts)
	{
		IAccount? account = accounts.FirstOrDefault()
			?? throw new System.NullReferenceException("Static data not yet extracted. Start the game and allow extraction of an account to access this data.");

		if (!account.TryGetApi<IGetAccountDataApi<T>>(out var api)
			|| !api.TryGetData(out T data))
			throw new System.NullReferenceException("Could not obtain value");

		return data;
	}
}

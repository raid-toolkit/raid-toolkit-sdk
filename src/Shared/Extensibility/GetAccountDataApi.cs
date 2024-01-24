using System.Diagnostics.CodeAnalysis;

namespace Raid.Toolkit.Extension;

public interface IGetAccountDataApi<T> where T : class
{
	bool TryGetData([NotNullWhen(true)] out T? data);
}

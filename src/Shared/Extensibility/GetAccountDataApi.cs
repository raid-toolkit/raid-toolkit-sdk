namespace Raid.Toolkit.Extension;

public interface IGetAccountDataApi<T> where T : class
{
	bool TryGetData(out T data);
}

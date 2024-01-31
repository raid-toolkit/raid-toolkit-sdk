using System.Diagnostics.CodeAnalysis;

namespace Raid.Toolkit.Extensibility.DataServices
{
	public interface IDataReader
	{
		bool TryRead<T>(IDataContext context, string key, [NotNullWhen(true)] out T? value) where T : class;
	}
}

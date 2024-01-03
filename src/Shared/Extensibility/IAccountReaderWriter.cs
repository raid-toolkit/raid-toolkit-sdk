using System.Diagnostics.CodeAnalysis;

namespace Raid.Toolkit.Extensibility;

public interface IAccountReaderWriter
{
	void Write<T>(string key, T value) where T : class;
	bool TryRead<T>(string key, [NotNullWhen(true)] out T? value) where T : class;
}

using System.Diagnostics.CodeAnalysis;

namespace Raid.Toolkit.Extensibility;

public interface IExtensionStorage
{
	bool TryRead<T>(string key, [NotNullWhen(true)] out T? value) where T : class;
	void Write<T>(string key, T value) where T : class;
}

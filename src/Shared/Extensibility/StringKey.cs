using Raid.Toolkit.DataModel;

namespace Raid.Toolkit.Extensibility;

public class StringKey
{
	private string? _value;
	public string Key { get; }
	public string? Value => _value ??= StaticResources.LocalizeByKey(Key);
	public bool HasValue => Value != null;

	public StringKey(string key)
	{
		Key = key;
	}
	public static implicit operator string?(StringKey? key) => key?.Value;
	public static implicit operator StringKey?(string? key) => key != null ? new(key) : null;
}

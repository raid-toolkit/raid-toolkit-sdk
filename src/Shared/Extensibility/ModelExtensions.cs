using System;
using System.Collections.Generic;
using System.Linq;

namespace Raid.Toolkit.Extensibility;

public static partial class ModelDictionaryExtensions
{
	public static IReadOnlyDictionary<string, V> ToModel<K, V>(this IDictionary<K, V> dict) where K : Enum
	{
		return dict == null ? new Dictionary<string, V>() : dict.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value);
	}

	public static IReadOnlyDictionary<K, V> ToModelEnum<K, V>(this IDictionary<K, V> dict) where K : Enum
	{
		return dict == null ? new Dictionary<K, V>() : dict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
	}
}

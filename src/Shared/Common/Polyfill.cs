using System.Collections.Generic;
#pragma warning disable IDE0005 // Using directive is unnecessary.
using System.Linq;
#pragma warning restore IDE0005 // Using directive is unnecessary.

namespace System
{
    public static class Net50Extensions
	{

#if NET5_0_OR_GREATER
        public static Dictionary<TKey, TValue> Filter<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> map, Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            return new(map.Where(predicate));
        }
#else
        public static Dictionary<TKey, TValue> Filter<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> map, Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            Dictionary<TKey, TValue> filteredCopy = new();
            foreach (var entry in map.Where(predicate))
                filteredCopy.Add(entry.Key, entry.Value);

            return filteredCopy;
        }

        public static bool IsAssignableTo(this Type type, Type assignableTo)
		{
			return assignableTo.IsAssignableFrom(type);
		}

        public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
                return false;
            dict.Add(key, value);
            return true;
        }

        public static bool Remove<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out TValue value)
        {
            if (dict.ContainsKey(key))
            {
                value = dict[key];
                dict.Remove(key);
                return true;
            }
            value = default;
            return false;
        }
#endif
    }
}

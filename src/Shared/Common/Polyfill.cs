#pragma warning disable IDE0005 // Using directive is unnecessary.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
#pragma warning restore IDE0005 // Using directive is unnecessary.

#if NET5_0_OR_GREATER
#else
namespace System.Runtime.CompilerServices
{
	public sealed class IsExternalInit
	{
	}
}
#endif

#if NET5_0_OR_GREATER
#else
namespace System.IO
{
	public static class Net50Extensions
	{
		public static Task<int> ReadAsync(this Stream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
		{
			return stream.ReadAsync(buffer.ToArray(), cancellationToken);
		}
		public static Task<int> ReadAsync(this Stream stream, byte[] buffer, CancellationToken cancellationToken = default)
		{
			return stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
		}
		public static Task WriteAsync(this Stream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
		{
			return stream.WriteAsync(buffer.ToArray(), cancellationToken);
		}
		public static Task WriteAsync(this Stream stream, byte[] buffer, CancellationToken cancellationToken = default)
		{
			return stream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
		}
	}
}
#endif

namespace System
{
	public static class Net50Extensions
	{

#if NET5_0_OR_GREATER
        public static Dictionary<TKey, TValue> Filter<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> map, Func<KeyValuePair<TKey, TValue>, bool> predicate) where TKey : notnull
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

		public static bool Remove<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out TValue? value)
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

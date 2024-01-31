using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Newtonsoft.Json;

using Raid.Toolkit.Common;

namespace Raid.Toolkit.Extensibility.DataServices
{
	public class CachedDataStorage : IDataStorage
	{
		private static readonly object EmptyObject = new();
		private static readonly ConcurrentDictionary<string, object> Cache = new();

		private readonly IDataStorage? UnderlyingStorage;

		public event EventHandler<DataStorageUpdatedEventArgs>? Updated;

		public static IEnumerable<string> GetKeys(IDataContext withinContext)
		{
			string prefix = $"{string.Join(";", withinContext.Parts)};";
			return Cache.Keys.ToArray()
				.Where(key => key.StartsWith(prefix))
				.Select(key => key.Split(';'))
				.Select(parts => parts[withinContext.Parts.Length])
				.Distinct();
		}

		public CachedDataStorage()
		{ }

		protected CachedDataStorage(IDataStorage underlyingStorage)
		{
			UnderlyingStorage = underlyingStorage;
		}

		public bool TryRead<T>(IDataContext context, string key, [NotNullWhen(true)] out T? value) where T : class
		{
			string cacheKey = string.Join(";", context.Parts.Concat(new[] { key }).ToArray());
			object cacheEntry = Cache.AddOrUpdate(cacheKey,
				cacheKey => ReadFromUnderlyingStorage<T>(context, key),
				(cacheKey, value) => value == EmptyObject ? ReadFromUnderlyingStorage<T>(context, key) : value);
			if (cacheEntry == EmptyObject)
			{
				value = default;
				return false;
			}
			value = (T)cacheEntry;
			return true;
		}

		public bool Write<T>(IDataContext context, string key, T value) where T : class
		{
			string cacheKey = string.Join(";", context.Parts.Concat(new[] { key }).ToArray());
			T? updatedValue = Cache.AddOrUpdate(cacheKey, _ => value, (_1, oldValue) => UpdateAndWriteIfChanged(oldValue, value)) as T;
			// only write if the new value was added
			if (updatedValue == value)
			{
				_ = (UnderlyingStorage?.Write(context, key, value));
				Updated?.Raise(this, new DataStorageUpdatedEventArgs(context, key, value));
				return true;
			}
			return false;
		}

		private object ReadFromUnderlyingStorage<T>(IDataContext context, string key) where T : class
		{
			return UnderlyingStorage != null && UnderlyingStorage.TryRead(context, key, out T? value) ? value : EmptyObject;
		}

		private static T UpdateAndWriteIfChanged<T>(T oldValue, T newValue)
		{
			// TODO: remove this extra serialization, and persist it directly as a string if possible
			return JsonConvert.SerializeObject(oldValue) == JsonConvert.SerializeObject(newValue) ? oldValue : newValue;
		}
	}

	public class CachedDataStorage<T> : CachedDataStorage where T : class, IDataStorage
	{
		public CachedDataStorage(T provider)
		: base(provider)
		{ }
	}
}

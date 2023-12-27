using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using Microsoft.Extensions.Options;

using Raid.Toolkit.Common;

namespace Raid.Toolkit.Extensibility.DataServices
{
	public class StorageSettings
	{
		public string InstallationPath { get; internal set; }
		public string StoragePath { get; internal set; }
		public StorageSettings()
		{
			InstallationPath = RegistrySettings.IsInstalled ? RegistrySettings.InstallationPath : ".";
			StoragePath = RegistrySettings.IsInstalled
				? Path.Combine(RegistrySettings.InstallationPath, "Data")
				: @".\Data";
		}
	}
	public class PersistedDataStorage : IDataStorage
	{
		private readonly string StoragePath;
		private readonly IDataStorageReaderWriter Storage;

		public PersistedDataStorage(IDataStorageReaderWriter storage, IOptions<StorageSettings> settings)
		{
			Storage = storage;
			StoragePath = settings.Value.StoragePath;
		}

		public event EventHandler<DataStorageUpdatedEventArgs>? Updated;

		public void Flush()
		{
			Storage.Flush();
		}

		public IEnumerable<string> GetKeys(IDataContext context)
		{
			string filePath = Path.Combine(StoragePath, Path.Combine(context.Parts));
			return Storage.GetKeys(filePath);
		}

		public bool TryRead<T>(IDataContext context, string key, [NotNullWhen(true)] out T? value) where T : class
		{
			string filePath = Path.Combine(StoragePath, Path.Combine(context.Parts), key);
			return Storage.TryRead(filePath, out value);
		}

		public bool Write<T>(IDataContext context, string key, T value) where T : class
		{
			string filePath = Path.Combine(StoragePath, Path.Combine(context.Parts), key);
			Updated?.Raise(this, new DataStorageUpdatedEventArgs(context, key, value));
			return Storage.Write(filePath, value);
		}
	}
}

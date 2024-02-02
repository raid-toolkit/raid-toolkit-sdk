using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Raid.Toolkit.Extensibility.DataServices;

namespace Raid.Toolkit.Extensibility.Host.Services
{
	public class FileStorageService : PollingBackgroundService, IDataStorageReaderWriter
	{
		private readonly static TimeSpan kPollInterval = new(0, 0, 5);
		private protected override TimeSpan PollInterval => kPollInterval;

		private readonly ConcurrentDictionary<string, object> PendingWrites = new();

		public FileStorageService(ILogger<FileStorageService> logger) : base(logger) { }

		protected override async Task ExecuteOnceAsync(CancellationToken token)
		{
			string[] filePaths = PendingWrites.Keys.ToArray();
			foreach (string filePath in filePaths)
			{
				if (!PendingWrites.TryRemove(filePath, out object? value))
					continue;

				string data = JsonConvert.SerializeObject(value);
				string? dir = Path.GetDirectoryName(filePath);
				if (dir == null)
				{
					Logger.LogError("Cannot determine parent directory of {filePath}. Cannot serialize file", filePath);
					continue;
				}
				Directory.CreateDirectory(dir);
				await File.WriteAllTextAsync(filePath, data, token);
			}
		}

		public IEnumerable<string> GetKeys(string filePath)
		{
			if (!Directory.Exists(filePath))
				return Array.Empty<string>();

			return Directory.GetDirectories(filePath).Select(entry => Path.GetFileName(entry));
		}

		public bool TryRead<T>(string filePath, [NotNullWhen(true)] out T? value) where T : class
		{
			if (PendingWrites.TryGetValue(filePath, out object? pendingValue) && pendingValue is T typedValue)
			{
				value = typedValue;
				return true;
			}

			if (!File.Exists(filePath))
			{
				value = default;
				return false;
			}
			try
			{
				value = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
				return value != null;
			}
			catch (Exception)
			{
				value = default;
				return false;
			}
		}

		public bool Write<T>(string filePath, T value) where T : class
		{
			_ = PendingWrites.AddOrUpdate(filePath, (_) => value, (_, _) => value);
			return true;
		}

		public void Flush()
		{
			string[] filePaths = PendingWrites.Keys.ToArray();
			foreach (string filePath in filePaths)
			{
				if (!PendingWrites.TryRemove(filePath, out object? value))
					continue;

				string data = JsonConvert.SerializeObject(value);
				string? dir = Path.GetDirectoryName(filePath);
				if (dir == null)
				{
					Logger.LogError("Cannot determine parent directory of {filePath}. Cannot serialize file", filePath);
					continue;
				}
				Directory.CreateDirectory(dir);
				File.WriteAllText(filePath, data);
			}
		}
	}
}

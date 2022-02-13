using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Raid.Service;

namespace Raid.DataServices
{
    public class FileStorage : PollingBackgroundService, IDataStorageReaderWriter
    {
        private readonly static TimeSpan kPollInterval = new(0, 0, 5);
        private protected override TimeSpan PollInterval => kPollInterval;

        private readonly ConcurrentDictionary<string, object> PendingWrites = new();

        public FileStorage(ILogger<FileStorage> logger): base(logger) { }

        protected override async Task ExecuteOnceAsync(CancellationToken token)
        {
            string[] filePaths = PendingWrites.Keys.ToArray();
            foreach (string filePath in filePaths)
            {
                if (!PendingWrites.TryRemove(filePath, out object value))
                    continue;

                string data = JsonConvert.SerializeObject(value);
                await File.WriteAllTextAsync(filePath, data, token);
            }
        }

        public bool TryRead<T>(string filePath, out T value) where T : class
        {
            if (PendingWrites.TryGetValue(filePath, out object pendingValue))
            {
                value = pendingValue as T;
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
                return true;
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
    }
}

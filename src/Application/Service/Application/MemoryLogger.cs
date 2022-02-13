using System;
using Il2CppToolkit.Runtime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Raid.Service
{
    public class MemoryLogger
    {
        private enum MemoryLoggingValue
        {
            None,
            Error,
            Access
        }
        private ILogger<MemoryLogger> Logger;
        private MemoryLoggingValue LoggingValue = MemoryLoggingValue.None;
        public MemoryLogger(ILogger<MemoryLogger> logger, IOptions<AppSettings> appSettings)
        {
            Logger = logger;
            Enum.TryParse<MemoryLoggingValue>(appSettings.Value.MemoryLogging, true, out LoggingValue);

            switch (LoggingValue)
            {
                case MemoryLoggingValue.Access:
                    MemorySourceExtensions.ObjectReadFromMemory += OnObjectReadFromMemory;
                    // passthrough
                    goto case MemoryLoggingValue.Error;
                case MemoryLoggingValue.Error:
                    MemorySourceExtensions.ObjectReadError += OnObjectReadError;
                    break;
                case MemoryLoggingValue.None:
                default:
                    break;
            }

        }

        private void OnObjectReadError(object sender, MemoryAccessErrorEventArgs e)
        {
            Logger.LogError(ServiceError.ObjectReadError.EventId(), e.Exception, $"Failed to read object '{e.Type.FullName}'@{e.Address:X8}");
        }

        private void OnObjectReadFromMemory(object sender, MemoryAccessEventArgs e)
        {
            Logger.LogInformation(ServiceEvent.ReadObject.EventId(), $"Reading object '{e.Type.FullName}'@{e.Address:X8}");
        }
    }
}

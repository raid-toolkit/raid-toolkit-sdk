using System;
using System.Text;
using Karambolo.Extensions.Logging.File;
using Microsoft.Extensions.Logging;

namespace Raid.Toolkit.Extensibility.Host.Utils
{
    public class SingleLineLogEntryTextBuilder : FileLogEntryTextBuilder
    {
        public static readonly SingleLineLogEntryTextBuilder Default = new();

        public override void BuildEntryText(StringBuilder sb, string categoryName, LogLevel logLevel, EventId eventId, string message, Exception exception, IExternalScopeProvider scopeProvider, DateTimeOffset timestamp)
        {
            // timestamp
            sb.Append(FormattableString.Invariant($"{timestamp.ToLocalTime():o}\t").PadRight(35, ' '));
            // process thread logLevel
            sb.Append($"{Environment.ProcessId,-5}\t{Environment.CurrentManagedThreadId,-5}\t{GetLogLevelString(logLevel),-10}\t");
            // event.id:name
            sb.Append($"{eventId.Id}\t");
            // category
            sb.Append($"{categoryName}\t");
            // message
            sb.Append($"{message}\t");
            // message
            if (exception != null)
                sb.Append($"{exception}\t");
            // scopes
            scopeProvider?.ForEachScope((scope, builder) => builder.Append($"\t[{scope}]"), sb);
            sb.AppendLine();
        }
    }
}

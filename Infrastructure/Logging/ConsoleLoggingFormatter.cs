using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace DotnetOrderService.Infrastructure.Logging
{
    public class ConsoleLoggingFormatter(IOptions<ConsoleFormatterOptions> options) : ConsoleFormatter("ConsoleLoggingFormatter")
    {
        private readonly ConsoleFormatterOptions _options = options.Value;
        
        private static readonly Dictionary<LogLevel, (ConsoleColor, string)> _logLevelConfigs = new()
        {
            [LogLevel.Trace] = (ConsoleColor.Gray, "\x1b[90m"),
            [LogLevel.Debug] = (ConsoleColor.Gray, "\x1b[90m"),
            [LogLevel.Information] = (ConsoleColor.Green, "\x1b[32m"),
            [LogLevel.Warning] = (ConsoleColor.Yellow, "\x1b[33m"),
            [LogLevel.Error] = (ConsoleColor.Red, "\x1b[31m"),
            [LogLevel.Critical] = (ConsoleColor.DarkRed, "\x1b[31m")
        };

        public override void Write<TState>(
            in LogEntry<TState> logEntry, 
            IExternalScopeProvider scopeProvider, 
            TextWriter textWriter
        )
        {
            string timestamp = DateTime.Now.ToString(_options.TimestampFormat);
            string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
            
            var (_, ansiColor) = _logLevelConfigs[logEntry.LogLevel];
            var resetCode = "\x1b[0m";
            
            textWriter.Write($"{ansiColor}{timestamp} [{logEntry.LogLevel}] {message}");
            
            if (logEntry.Exception != null)
            {
                textWriter.WriteLine(logEntry.Exception.ToString());
            }
            
            textWriter.Write(resetCode);
            textWriter.WriteLine();
        }
    }
}

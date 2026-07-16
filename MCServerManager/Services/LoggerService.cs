using System;
using System.Text.RegularExpressions;
using MCServerManager.Models;

namespace MCServerManager.Services;

/// <summary>
/// Interface for the service that manages the logs of the server
/// </summary>
public partial class LoggerService : ILoggerService, IDisposable
{
    public event EventHandler<Log>? LogOutput;

    // [hh:mm:ss] [thread/LEVEL]: MESSAGE
    [GeneratedRegex(@"^\[\d{1,2}:\d{1,2}:\d{1,2}\] \[[^\]]*/(?<level>INFO|WARN|ERROR|DEBUG|TRACE|FATAL)\]: (?<msg>.*)$")]
    private static partial Regex LogRegex();

    public void ParseLine(string line)
    {
        var match = LogRegex().Match(line);

        if (!match.Success)
            return;

        string message = match.Groups["msg"].Value;

        LogType logType = match.Groups["level"].Value switch
        {
            "INFO" => LogType.Info,
            "WARN" => LogType.Warning,
            "ERROR" => LogType.Error,
            "DEBUG" => LogType.Debug,
            "TRACE" => LogType.Trace,
            "FATAL" => LogType.Fatal,
            _ => LogType.Unknown
        };

        Log log = new()
        {
            RawContent = line,
            Type = logType,
            Message = message
        };

        LogOutput?.Invoke(this, log);
    }

    public void Dispose() { }
}

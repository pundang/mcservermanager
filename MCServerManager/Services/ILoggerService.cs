using System;
using MCServerManager.Models;

namespace MCServerManager.Services;

/// <summary>
/// Interface for the service that manages the logs of the server
/// </summary>
public interface ILoggerService
{
    event EventHandler<Log>? LogOutput;

    void ParseLine(string log);
}

using System;
using System.Threading.Tasks;
using MCServerManager.Models;

namespace MCServerManager.Services;

/// <summary>
/// Interface for the service that manages the Minecraft server process
/// </summary>
public interface IServerProcessService
{
    ServerProcessInfo Info { get; }

    event EventHandler<string>? OutputReceived; // Raw stdout/stderr lines
    event EventHandler<ServerStatus>? StatusChanged;

    Task StartAsync(string workingDirectory, string javaArgs = "-Xmx2G -jar server.jar nogui");
    Task StopAsync(TimeSpan? gracefulTimeout = null);
    Task RestartAsync();
    Task SendCommandAsync(string command);
}

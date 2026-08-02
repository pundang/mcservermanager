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
    int MaxMemory { get; set; }

    event EventHandler<string>? OutputReceived; // Raw stdout/stderr lines
    event EventHandler<ServerStatus>? StatusChanged;
    event EventHandler<ResourceUsage>? ResourceUsageChanged;

    Task StartAsync(string workingDirectory, int maxMemory = 2048, string javaArgs = "-jar server.jar nogui");
    Task StopAsync(TimeSpan? gracefulTimeout = null);
    Task RestartAsync();
    Task SendCommandAsync(string command);
}

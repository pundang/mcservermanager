using System;
using System.Threading.Tasks;

namespace MCServerManager.Services;

public interface IServerProcessService
{
    ServerProcessInfo Info { get; }

    event EventHandler<string>? OutputReceived; // raw stdout/stderr lines
    event EventHandler<ServerStatus>? StatusChanged;

    Task StartAsync(string workingDirectory, string javaArgs = "-Xmx2G -jar server.jar nogui");
    Task StopAsync(TimeSpan? gracefulTimeout = null);
    Task RestartAsync();
    Task SendCommandAsync(string command);
}

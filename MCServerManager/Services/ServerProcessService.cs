using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using MCServerManager.Models;

namespace MCServerManager.Services;

/// <summary>
/// Launches and supervises the Minecraft server as a child process, piping
/// stdin/stdout for command execution and log capture, and polling basic
/// resource stats (uptime, memory) on a timer while running.
/// </summary>
public class ServerProcessService : IServerProcessService, IDisposable
{
    private Process? _process;
    private readonly DispatcherTimer _statsTimer;

    public ServerProcessInfo Info { get; } = new();

    public event EventHandler<string>? OutputReceived;
    public event EventHandler<ServerStatus>? StatusChanged;

    public ServerProcessService()
    {
        _statsTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _statsTimer.Tick += (_, _) => UpdateStats();
    }

    /// <summary>
    /// Starts the server by executing a command
    /// </summary>
    public Task StartAsync(string workingDirectory, string javaArgs = "-Xmx2G -jar server.jar nogui")
    {
        if (_process is { HasExited: false })
            return Task.CompletedTask;

        SetStatus(ServerStatus.Starting);

        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = javaArgs,
                WorkingDirectory = workingDirectory,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };

        // Pipe stdin and stderr to the event handler
        _process.OutputDataReceived += (_, e) => { if (e.Data is not null) OutputReceived?.Invoke(this, e.Data); };
        _process.ErrorDataReceived += (_, e) => { if (e.Data is not null) OutputReceived?.Invoke(this, e.Data); };
        _process.Exited += OnProcessExited;

        _process.Start();
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

        Info.ProcessId = _process.Id;
        Info.StartedAt = DateTime.Now;
        SetStatus(ServerStatus.Running);
        _statsTimer.Start();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Tries to stop the server through stdin
    /// If after 30s the server isn't closed it kills the server process
    /// </summary>
    public async Task StopAsync(TimeSpan? gracefulTimeout = null)
    {
        if (_process is null || _process.HasExited) return;

        SetStatus(ServerStatus.Stopping);
        await SendCommandAsync("stop");

        var timeout = gracefulTimeout ?? TimeSpan.FromSeconds(30);
        using var cts = new CancellationTokenSource(timeout);

        try
        {
            await _process.WaitForExitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            _process.Kill(entireProcessTree: true);
        }
    }

    /// <summary>
    /// Restarts the server process
    /// </summary>
    public async Task RestartAsync()
    {
        var workingDir = _process?.StartInfo.WorkingDirectory ?? "";
        await StopAsync();
        await StartAsync(workingDir);
    }

    /// <summary>
    /// Send a command through stdin that the server will receive
    /// </summary>
    public Task SendCommandAsync(string command)
    {
        if (_process is null || _process.HasExited)
            return Task.CompletedTask;

        return _process.StandardInput.WriteLineAsync(command);
    }

    /// <summary>
    /// Event handler for the server process exit
    /// </summary>
    private void OnProcessExited(object? sender, EventArgs e)
    {
        _statsTimer.Stop();
        SetStatus(_process?.ExitCode == 0 ? ServerStatus.Stopped : ServerStatus.Crashed);
    }

    /// <summary>
    /// Update the stats for the server process uptime and memory
    /// </summary>
    private void UpdateStats()
    {
        if (_process is null || _process.HasExited) return;

        _process.Refresh();
        Info.Uptime = DateTime.Now - (Info.StartedAt ?? DateTime.Now);
        Info.MemoryUsageMb = _process.WorkingSet64 / 1024 / 1024;
    }

    /// <summary>
    /// Sets the status for the server process
    /// Stopped | Starting | Running | Stopping | Crashed
    /// </summary>
    private void SetStatus(ServerStatus status)
    {
        Info.Status = status;
        StatusChanged?.Invoke(this, status);
    }

    public void Dispose()
    {
        _statsTimer.Stop();
        _process?.Dispose();
        GC.SuppressFinalize(this);
    }
}

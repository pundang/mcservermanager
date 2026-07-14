using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace MCServerManager.Services;

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

    public async Task StopAsync(TimeSpan? gracefulTimeout = null)
    {
        if (_process is null || _process.HasExited) return;

        SetStatus(ServerStatus.Stopping);
        await SendCommandAsync("stop");

        var timeout = gracefulTimeout ?? TimeSpan.FromSeconds(30);
        var exited = _process.WaitForExit((int)timeout.TotalMilliseconds);

        if (!exited)
            _process.Kill(entireProcessTree: true);
    }

    public async Task RestartAsync()
    {
        var workingDir = _process?.StartInfo.WorkingDirectory ?? "";
        await StopAsync();
        await StartAsync(workingDir);
    }

    public Task SendCommandAsync(string command)
    {
        if (_process is null || _process.HasExited)
            return Task.CompletedTask;

        return _process.StandardInput.WriteLineAsync(command);
    }

    private void OnProcessExited(object? sender, EventArgs e)
    {
        _statsTimer.Stop();
        SetStatus(_process?.ExitCode == 0 ? ServerStatus.Stopped : ServerStatus.Crashed);
    }

    private void UpdateStats()
    {
        if (_process is null || _process.HasExited) return;

        _process.Refresh();
        Info.Uptime = DateTime.Now - (Info.StartedAt ?? DateTime.Now);
        Info.MemoryUsageMb = _process.WorkingSet64 / 1024 / 1024;
    }

    private void SetStatus(ServerStatus status)
    {
        Info.Status = status;
        StatusChanged?.Invoke(this, status);
    }

    public void Dispose()
    {
        _statsTimer.Stop();
        _process?.Dispose();
    }
}

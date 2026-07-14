using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MCServerManager.Services;

namespace MCServerManager.ViewModels;

/// <summary>
/// Main ViewModel for managing the server configuration and runtime
/// </summary>
public partial class MainViewModel : ObservableObject
{
    // Service for managing the server process
    private readonly IServerProcessService _processService;

    public ServerProcessInfo ServerInfo => _processService.Info;

    public bool IsRunning => ServerInfo.Status is ServerStatus.Running or ServerStatus.Starting;
    public string ToggleButtonText => ServerInfo.Status switch
    {
        ServerStatus.Running => "Stop",
        ServerStatus.Starting => "Starting...",
        ServerStatus.Stopping => "Stopping...",
        _ => "Start"
    };
    public bool ToggleButtonEnabled => ServerInfo.Status switch
    {
        ServerStatus.Stopped or ServerStatus.Running => true,
        ServerStatus.Starting or ServerStatus.Stopping => false,
        _ => false // We shouldn't reach this point
    };

    public MainViewModel(IServerProcessService processService)
    {
        // Send signals to UI when the process changes status
        _processService = processService;
        _processService.StatusChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsRunning));
            OnPropertyChanged(nameof(ToggleButtonText));
        };
    }

    /// <summary>
    /// Runs and shuts off the server
    /// </summary>
    [RelayCommand]
    private async Task ToggleRunningAsync()
    {
        if (IsRunning)
            await _processService.StopAsync();
        else
            await _processService.StartAsync(Directory.GetCurrentDirectory() + "/server/"); // TODO: FIX HARDCODED PATH
    }
}

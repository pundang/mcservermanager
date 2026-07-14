using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MCServerManager.Services;

namespace MCServerManager.ViewModels;

public partial class MainViewModel : ObservableObject
{
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

    public MainViewModel(IServerProcessService processService)
    {
        _processService = processService;
        _processService.StatusChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsRunning));
            OnPropertyChanged(nameof(ToggleButtonText));
        };
    }

    [RelayCommand]
    private async Task ToggleRunningAsync()
    {
        if (IsRunning)
            await _processService.StopAsync();
        else
            await _processService.StartAsync(Directory.GetCurrentDirectory() + "/server/"); // TODO: FIX HARDCODED PATH
    }
}

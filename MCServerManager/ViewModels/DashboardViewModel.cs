using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MCServerManager.Services;

namespace MCServerManager.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    readonly IStorageManagerService _storageManagerService;
    readonly IServerProcessService _serverProcessService;

    [ObservableProperty]
    public partial float CpuUsage { get; set; }
    [ObservableProperty]
    public partial float RamUsage { get; set; }
    [ObservableProperty]
    public partial float RamUsagePercentage { get; set; }

    public DashboardViewModel(IStorageManagerService storageManagerService, IServerProcessService serverProcessService)
    {
        _storageManagerService = storageManagerService;
        _serverProcessService = serverProcessService;

        _serverProcessService.ResourceUsageChanged += (_, usage) =>
        {
            CpuUsage = usage.Cpu;
            RamUsage = usage.Ram;
            RamUsagePercentage = _serverProcessService.MaxMemory > 0
                ? (float)usage.Ram / _serverProcessService.MaxMemory * 100f
                : 0f;
        };
    }

    [RelayCommand]
    public void OpenServerInExplorer()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = _storageManagerService.ServerDirectory,
            UseShellExecute = true
        });
    }
}

using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MCServerManager.Services;

namespace MCServerManager.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    const int HistoryLength = 60;

    readonly IStorageManagerService _storageManagerService;
    readonly IServerProcessService _serverProcessService;

    [ObservableProperty]
    public partial float CpuUsage { get; set; }
    [ObservableProperty]
    public partial float RamUsage { get; set; }
    [ObservableProperty]
    public partial float RamUsagePercentage { get; set; }

    public ObservableCollection<float> CpuHistory { get; } = [];
    public ObservableCollection<float> RamHistory { get; } = [];

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

            Track(CpuHistory, CpuUsage);
            Track(RamHistory, RamUsagePercentage);
        };
    }

    static void Track(ObservableCollection<float> history, float value)
    {
        history.Add(value);
        if (history.Count > HistoryLength) history.RemoveAt(0);
    }

    [RelayCommand]
    public void OpenServerInExplorer() => Process.Start(new ProcessStartInfo
    {
        FileName = _storageManagerService.ServerDirectory,
        UseShellExecute = true
    });
}

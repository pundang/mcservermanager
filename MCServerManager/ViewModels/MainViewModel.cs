using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MCServerManager.Models;
using MCServerManager.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MCServerManager.ViewModels;

/// <summary>
/// Main ViewModel for managing the server configuration and runtime
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IServiceProvider _provider;
    private readonly IServerProcessService _processService;
    private readonly IStorageManagerService _storageManagerService;

    [ObservableProperty]
    public partial ViewModelBase CurrentViewModel { get; set; }

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

    public MainViewModel(IServiceProvider serviceProvider, IServerProcessService processService, IStorageManagerService storageManagerService)
    {
        _provider = serviceProvider;
        _processService = processService;
        _storageManagerService = storageManagerService;
        CurrentViewModel = _provider.GetRequiredService<DashboardViewModel>();

        // Send signals to UI when the process changes status
        _processService.StatusChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsRunning));
            OnPropertyChanged(nameof(ToggleButtonText));
            OnPropertyChanged(nameof(ToggleButtonEnabled));
        };
    }

    /// <summary>
    /// Tab switch function
    /// </summary>
    [RelayCommand]
    private void SelectTab(string tab) =>
    CurrentViewModel = tab switch
    {
        "Dashboard" => _provider.GetRequiredService<DashboardViewModel>(),
        "Console" => _provider.GetRequiredService<ConsoleViewModel>(),
        "Settings" => _provider.GetRequiredService<ServerSettingsViewModel>(),
        "Versions" => _provider.GetRequiredService<VersionsViewModel>(),
        _ => CurrentViewModel
    };

    /// <summary>
    /// Runs and shuts off the server
    /// </summary>
    [RelayCommand]
    private async Task ToggleRunningAsync()
    {
        if (IsRunning)
            await _processService.StopAsync();
        else
            await _processService.StartAsync(_storageManagerService.ServerDirectory);
    }
}

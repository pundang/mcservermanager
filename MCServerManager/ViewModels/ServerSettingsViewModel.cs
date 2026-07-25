using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MCServerManager.Models;
using MCServerManager.Services;

namespace MCServerManager.ViewModels;

public partial class ServerSettingsViewModel : ViewModelBase
{
    private readonly IServerSettingsService _serverSettingsService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ServerSettingsIsEmpty))]
    public partial List<ServerSetting> ServerSettings { get; set; } = [];
    public bool ServerSettingsIsEmpty => ServerSettings.Count == 0;

    public ServerSettingsViewModel(IServerSettingsService serverSettingsService)
    {
        _serverSettingsService = serverSettingsService;
        _ = LoadSettingsAsync();
    }

    /// <summary>
    /// Load settings
    /// </summary>
    [RelayCommand]
    private async Task LoadSettingsAsync()
    {
        ServerSettings = await _serverSettingsService.LoadSettings();
    }

    /// <summary>
    /// Saves the settings to the server
    /// </summary>
    [RelayCommand]
    private async Task SaveSettings()
    {
        await _serverSettingsService.SaveSettings(ServerSettings);
    }
}

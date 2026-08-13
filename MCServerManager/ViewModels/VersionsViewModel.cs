using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MCServerManager.Models;
using MCServerManager.Services;

namespace MCServerManager.ViewModels;

public partial class VersionsViewModel : ViewModelBase
{
    private readonly IVersionManagerService _versionManagerService;

    [ObservableProperty]
    public partial bool ManifestDownloaded { get; set; } = false;
    [ObservableProperty]
    public partial bool ManifestDownloadError { get; set; } = false;
    [ObservableProperty]
    public partial string ManifestDownloadErrorMessage { get; set; } = "Unknown";

    public List<VersionItemViewModel> Versions { get; set; } = [];
    [ObservableProperty]
    public partial ObservableCollection<VersionItemViewModel> FilteredVersions { get; set; } = [];
    [ObservableProperty]
    public partial string VersionChannel { get; set; } = "Unknown";

    public VersionsViewModel(IVersionManagerService versionManagerService)
    {
        _versionManagerService = versionManagerService;
        _ = LoadVersionsAsync();
    }

    [RelayCommand]
    private async Task LoadVersionsAsync()
    {
        // If user re-tries to download manifest
        ManifestDownloadError = false;
        ManifestDownloadErrorMessage = "Unknown";

        try
        {
            if (_versionManagerService.VersionManifest is null)
                await _versionManagerService.DownloadManifest();
        }
        catch (Exception ex)
        {
            ManifestDownloadError = true;
            ManifestDownloadErrorMessage = ex.Message;
            return;
        }

        Versions = [.. _versionManagerService.VersionManifest!.Versions
        .Select(v => new VersionItemViewModel(v, _versionManagerService))];

        await FilterVersionsByType("Release");

        ManifestDownloaded = true;
    }

    [RelayCommand]
    public async Task FilterVersionsByType(string type)
    {
        VersionChannel = type;

        VersionBaseType versionType = type switch
        {
            "Release" => VersionBaseType.Release,
            "Snapshot" => VersionBaseType.Snapshot,
            "Old Beta" => VersionBaseType.OldBeta,
            "Old Alpha" => VersionBaseType.OldAlpha,
            _ => VersionBaseType.Release
        };

        if (Versions.Count == 0)
            await LoadVersionsAsync();

        FilteredVersions = new ObservableCollection<VersionItemViewModel>(
            Versions.Where(vm => vm.Version.Type == versionType)
        );
    }
}

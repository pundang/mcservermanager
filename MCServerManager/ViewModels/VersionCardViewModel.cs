using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MCServerManager.Models;
using MCServerManager.Services;

namespace MCServerManager.ViewModels;

public partial class VersionItemViewModel(VersionBase version, IVersionManagerService versionManagerService) : ViewModelBase
{
    private readonly IVersionManagerService _versionManagerService = versionManagerService;

    public VersionBase Version { get; } = version;

    [ObservableProperty]
    public partial bool InstallButtonEnabled { get; set; } = true;
    [ObservableProperty]
    public partial string InstallButtonText { get; set; } = "Install";

    [RelayCommand]
    public async Task InstallVersion(string versionId)
    {
        InstallButtonText = "Installing...";
        InstallButtonEnabled = false;

        DownloadResult downloadable = await _versionManagerService.DownloadVersionBinary(versionId);

        InstallButtonText = downloadable switch
        {
            DownloadResult.Success => "Installed",
            DownloadResult.VersionNotFound => "Not found",
            DownloadResult.NoServerJarAvailable => "Not available",
            DownloadResult.DownloadFailed => "Failed",
            _ => "Unknown"
        };

        await Task.Delay(2000);

        InstallButtonText = "Install";
        InstallButtonEnabled = true;
    }
}

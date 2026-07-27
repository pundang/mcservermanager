using CommunityToolkit.Mvvm.Input;
using MCServerManager.Models;
using MCServerManager.Services;

namespace MCServerManager.ViewModels;

public partial class VersionItemViewModel(VersionBase version, IVersionManagerService versionManagerService) : ViewModelBase
{
    private readonly IVersionManagerService _versionManagerService = versionManagerService;

    public VersionBase Version { get; } = version;

    [RelayCommand]
    public void InstallVersion(string versionId)
    {
        _versionManagerService.DownloadVersion(versionId);
    }
}

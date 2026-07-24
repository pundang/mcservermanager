using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MCServerManager.Models;
using MCServerManager.Services;

namespace MCServerManager.ViewModels;

public partial class VersionItemViewModel : ViewModelBase
{
    private readonly IVersionManagerService _versionManagerService;

    public VersionBase Version { get; }

    public VersionItemViewModel(VersionBase version, IVersionManagerService versionManagerService)
    {
        _versionManagerService = versionManagerService;
        Version = version;
    }

    [RelayCommand]
    public void InstallVersion(string versionId)
    {
        _versionManagerService.DownloadVersion(versionId);
    }
}

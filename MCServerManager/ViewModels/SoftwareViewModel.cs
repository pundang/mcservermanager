using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MCServerManager.Services;

namespace MCServerManager.ViewModels;

public partial class SoftwareViewModel : ViewModelBase
{
    private readonly IVersionManagerService _versionManagerService;

    public ObservableCollection<VersionItemViewModel> Versions { get; } = [];

    public SoftwareViewModel(IVersionManagerService versionManagerService)
    {
        _versionManagerService = versionManagerService;
        _ = LoadVersionsAsync();
    }

    private async Task LoadVersionsAsync()
    {
        if (_versionManagerService.VersionManifest is null)
            await _versionManagerService.GetManifest();

        foreach (var version in _versionManagerService.VersionManifest!.Versions)
            Versions.Add(new VersionItemViewModel(version, _versionManagerService));
    }
}

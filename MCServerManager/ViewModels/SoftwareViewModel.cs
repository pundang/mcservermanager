using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MCServerManager.Services;

namespace MCServerManager.ViewModels;

public partial class SoftwareViewModel : ViewModelBase
{
    private readonly IVersionManagerService _versionManagerService;

    [ObservableProperty]
    public partial bool ManifestDownloaded { get; set; } = false;

    [ObservableProperty]
    public partial ObservableCollection<VersionItemViewModel> Versions { get; set; } = [];

    public SoftwareViewModel(IVersionManagerService versionManagerService)
    {
        _versionManagerService = versionManagerService;
        _ = LoadVersionsAsync();
    }

    private async Task LoadVersionsAsync()
    {
        if (_versionManagerService.VersionManifest is null)
            await _versionManagerService.GetManifest();

        Versions = new ObservableCollection<VersionItemViewModel>(
            _versionManagerService.VersionManifest!.Versions
                .Select(v => new VersionItemViewModel(v, _versionManagerService)));

        ManifestDownloaded = true;
    }
}

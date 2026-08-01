using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using MCServerManager.Services;

namespace MCServerManager.ViewModels;

public partial class DashboardViewModel(IStorageManagerService storageManagerService) : ViewModelBase
{
    readonly IStorageManagerService _storageManagerService = storageManagerService;

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

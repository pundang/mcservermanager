using CommunityToolkit.Mvvm.ComponentModel;

namespace MCServerManager.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}

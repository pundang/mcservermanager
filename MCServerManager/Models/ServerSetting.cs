using CommunityToolkit.Mvvm.ComponentModel;

namespace MCServerManager.Models;

public partial class ServerSetting : ObservableObject
{
    [ObservableProperty]
    public partial string Key { get; set; }

    [ObservableProperty]
    public partial string Value { get; set; }
}

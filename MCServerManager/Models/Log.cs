using CommunityToolkit.Mvvm.ComponentModel;

namespace MCServerManager.Models;

public partial class Log : ObservableObject
{
    [ObservableProperty]
    public partial string RawContent { get; set; } = "";

    [ObservableProperty]
    public partial LogType Type { get; set; } = LogType.Unknown;

    [ObservableProperty]
    public partial string Message { get; set; } = "";
}

public enum LogType { Unknown, Info, Warning, Error, Debug, Trace, Fatal }

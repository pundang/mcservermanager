using CommunityToolkit.Mvvm.ComponentModel;

public partial class Log : ObservableObject
{
    [ObservableProperty]
    private string rawContent = "";

    [ObservableProperty]
    private LogType type = LogType.Unknown;

    [ObservableProperty]
    private string message = "";
}

public enum LogType { Unknown, Info, Warning, Error, Debug, Trace, Fatal }

using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MCServerManager.Models;

public partial class ServerProcessInfo : ObservableObject
{
    [ObservableProperty]
    private ServerStatus status = ServerStatus.Stopped;

    [ObservableProperty]
    private int processId;

    [ObservableProperty]
    private TimeSpan uptime;

    [ObservableProperty]
    private long memoryUsageMb;

    [ObservableProperty]
    private DateTime? startedAt;
}

public enum ServerStatus { Stopped, Starting, Running, Stopping, Crashed }

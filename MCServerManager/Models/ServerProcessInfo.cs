using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MCServerManager.Models;

public partial class ServerProcessInfo : ObservableObject
{
    [ObservableProperty]
    public partial ServerStatus Status { get; set; } = ServerStatus.Stopped;

    [ObservableProperty]
    public partial int ProcessId { get; set; }

    [ObservableProperty]
    public partial TimeSpan Uptime { get; set; }

    [ObservableProperty]
    public partial long MemoryUsageMb { get; set; }

    [ObservableProperty]
    public partial DateTime? StartedAt { get; set; }
}

public enum ServerStatus { Stopped, Starting, Running, Stopping, Crashed }

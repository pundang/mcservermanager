using CommunityToolkit.Mvvm.ComponentModel;

namespace MCServerManager.Models;

public partial class ResourceUsage : ObservableObject
{
    public float Cpu { get; set; }
    public float Ram { get; set; }
}

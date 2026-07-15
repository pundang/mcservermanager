using MCServerManager.Services;

namespace MCServerManager.ViewModels;

/// <summary>
/// Design ViewModel for the main server configuration View with mock data
/// </summary>
public partial class DesignMainViewModel : MainViewModel
{
    public DesignMainViewModel() : base(new ServerProcessService())
    {
        ServerInfo.Status = ServerStatus.Stopped;
    }
}

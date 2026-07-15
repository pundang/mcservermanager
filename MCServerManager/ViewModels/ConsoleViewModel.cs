using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using MCServerManager.Services;

namespace MCServerManager.ViewModels;

public partial class ConsoleViewModel : ViewModelBase
{
    private readonly IServerProcessService _processService;

    public ObservableCollection<string> ConsoleOutputLines { get; } = [];

    public ConsoleViewModel(IServerProcessService processService)
    {
        _processService = processService;
        _processService.OutputReceived += (_, line) => ConsoleOutputLines.Add(line);
    }

    /// <summary>
    /// Send command to the running server
    /// </summary>
    [RelayCommand]
    private void SendCommand(string command)
    {
        _processService.SendCommandAsync(command);
    }
}

using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using MCServerManager.Services;

namespace MCServerManager.ViewModels;

public partial class ConsoleViewModel : ViewModelBase
{
    private readonly IServerProcessService _processService;
    private readonly ILoggerService _loggerService;

    public ObservableCollection<string> ConsoleOutputLines { get; } = [];

    public ConsoleViewModel(IServerProcessService processService, ILoggerService loggerService)
    {
        _processService = processService;
        _processService.OutputReceived += (_, line) => loggerService.ParseLine(line);

        _loggerService = loggerService;
        _loggerService.LogOutput += (_, log) =>
        {
            ConsoleOutputLines.Add(log.RawContent);
        };
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

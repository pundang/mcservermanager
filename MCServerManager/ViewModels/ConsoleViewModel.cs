using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using MCServerManager.Services;
using Microsoft.Extensions.Logging;

namespace MCServerManager.ViewModels;

public partial class ConsoleViewModel : ViewModelBase
{
    private readonly ILogger<ConsoleViewModel> _logger;
    private readonly IServerProcessService _processService;
    private readonly ILoggerService _loggerService;

    public ObservableCollection<string> ConsoleOutputLines { get; } = [];

    public ConsoleViewModel(ILogger<ConsoleViewModel> logger, IServerProcessService processService, ILoggerService loggerService)
    {
        _logger = logger;
        _logger.LogDebug("ConsoleViewModel ctor");

        _processService = processService;
        _processService.OutputReceived += (_, line) => loggerService.CreateLogFromString(line);

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

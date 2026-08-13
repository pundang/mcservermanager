using System;
using System.IO;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MCServerManager.Services;
using Microsoft.Extensions.Logging;

namespace MCServerManager.ViewModels;

public partial class EulaViewModel : ViewModelBase
{
    readonly ILogger<EulaViewModel> _logger;
    readonly IStorageManagerService _storageManagerService;

    [ObservableProperty]
    public partial string EulaFileContent { get; set; }

    [ObservableProperty]
    public partial string EulaPrompt { get; set; } = "By continuing, you agree to Mojang's EULA (https://aka.ms/MinecraftEULA). Accept?";
    [GeneratedRegex(@"eula=(?<value>.*)$")]
    private partial Regex EulaAgreementRegex();

    public EulaViewModel(ILogger<EulaViewModel> logger, IStorageManagerService storageManagerService)
    {
        _logger = logger;
        _storageManagerService = storageManagerService;

        try
        {
            string content = _storageManagerService.LoadFileAsString(
                Path.Combine(_storageManagerService.ServerDirectory, "eula.txt")
            );

            EulaFileContent = content;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to load EULA file: {exception}", ex.Message);
            throw;
        }
    }

    private string ModifyEulaState(bool accept)
    {
        string newValue = accept ? "true" : "false";

        string updatedContent = EulaAgreementRegex().Replace(EulaFileContent, $"eula={newValue}");

        string eulaPath = Path.Combine(_storageManagerService.ServerDirectory, "eula.txt");
        _storageManagerService.SaveFileFromString(eulaPath, updatedContent);

        EulaFileContent = updatedContent;
        return updatedContent;
    }

    [RelayCommand]
    public void AcceptEula() => ModifyEulaState(true);

    [RelayCommand]
    public void RejectEula() => ModifyEulaState(false);
}

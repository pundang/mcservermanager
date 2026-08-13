using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MCServerManager.Models;

namespace MCServerManager.Services;

public partial class ServerSettingsService(IStorageManagerService storageManagerService) : IServerSettingsService
{
    private readonly IStorageManagerService _storageManagerService = storageManagerService;

    // key=value
    [GeneratedRegex(@"(?<key>.*)=(?<value>.*)")]
    private static partial Regex SettingRegex();

    public async Task<List<ServerSetting>> LoadSettings()
    {
        var settings = new List<ServerSetting>();

        string settingsString = await _storageManagerService.LoadFileAsStringAsync(
            Path.Combine(_storageManagerService.ServerDirectory, "server.properties")
        );

        foreach (Match match in SettingRegex().Matches(settingsString))
        {
            if (!match.Success)
            {
                continue;
            }

            settings.Add(new ServerSetting
            {
                Key = match.Groups["key"].Value,
                Value = match.Groups["value"].Value,
            });
        }

        return settings;
    }

    public async Task SaveSettings(List<ServerSetting> serverSettingsObject)
    {
        string content = string.Join("\n",
            serverSettingsObject.Select(setting => $"{setting.Key}={setting.Value}")
        ) + "\n";

        await _storageManagerService.SaveFileFromStringAsync(
            Path.Combine(_storageManagerService.ServerDirectory, "server.properties"),
            content
        );

        return;
    }
}

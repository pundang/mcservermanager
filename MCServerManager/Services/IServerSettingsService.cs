using System.Collections.Generic;
using System.Threading.Tasks;
using MCServerManager.Models;

namespace MCServerManager.Services;

/// <summary>
/// Interface for the service that manages server settings
/// </summary>
public interface IServerSettingsService
{
    Task<List<ServerSetting>> LoadSettings();
    Task SaveSettings(List<ServerSetting> serverSettingsObject);
}

using System.Threading.Tasks;
using MCServerManager.Models;

namespace MCServerManager.Services;

/// <summary>
/// Interface for the service that manages the versions
/// </summary>
public interface IVersionManagerService
{
    public Manifest? VersionManifest { get; set; }

    Task GetManifest();
    Task DownloadVersion(string versionId);
}

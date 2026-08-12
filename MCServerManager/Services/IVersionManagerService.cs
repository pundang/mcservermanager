using System.Threading.Tasks;
using MCServerManager.Models;

namespace MCServerManager.Services;

/// <summary>
/// Interface for the service that manages the versions
/// </summary>
public interface IVersionManagerService
{
    Manifest? VersionManifest { get; set; }

    Task<Manifest?> DownloadManifest();
    Task<DownloadResult> DownloadVersionBinary(string versionId);
}

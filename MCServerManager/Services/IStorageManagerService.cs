using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MCServerManager.Services;

/// <summary>
/// Interface for the service that manages server storage
/// </summary>
public interface IStorageManagerService
{
    string ServerDirectory { get; }
    string ServerJarPath { get; }

    Task DownloadOrReplaceServerJarAsync(Stream content, CancellationToken cancellationToken = default);
}

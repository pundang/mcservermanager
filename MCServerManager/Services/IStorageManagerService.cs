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
    string RootDirectory { get; }

    Task DownloadOrReplaceServerJarAsync(Stream content, CancellationToken cancellationToken = default);
    string LoadFileAsString(string path);
    Task<string> LoadFileAsStringAsync(string path, CancellationToken cancellationToken = default);
    void SaveFileFromString(string filePath, string content);
    Task SaveFileFromStringAsync(string filePath, string content, CancellationToken cancellationToken = default);
}

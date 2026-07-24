using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MCServerManager.Services;

/// <summary>
/// Service for managing server files
/// </summary>
public class StorageManagerService : IStorageManagerService
{
    public string ServerDirectory { get; }
    public string ServerJarPath { get; }

    private const int RandomAccessBufferSize = 81920;

    public StorageManagerService()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string root = Path.Combine(appData, "MCServerManager");

        string serverPath = Path.Combine(root, "server");
        string jarPath = Path.Combine(serverPath, "server.jar");

        ServerDirectory = serverPath;
        ServerJarPath = jarPath;
    }

    public async Task DownloadOrReplaceServerJarAsync(Stream content, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(ServerDirectory);

        string temporaryPath = Path.Combine(
            ServerDirectory,
            $"{Path.GetFileName(ServerJarPath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            await using (FileStream temporaryFile = new(
                temporaryPath,
                FileMode.OpenOrCreate,
                FileAccess.Write,
                FileShare.None,
                bufferSize: RandomAccessBufferSize,
                options: FileOptions.SequentialScan | FileOptions.Asynchronous))
            {
                await content.CopyToAsync(temporaryFile, cancellationToken);
            }

            File.Move(temporaryPath, ServerJarPath, overwrite: true);
        }
        finally
        {
            File.Delete(temporaryPath);
        }
    }
}

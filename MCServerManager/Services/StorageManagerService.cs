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
    public string RootDirectory { get; }

    private const int RandomAccessBufferSize = 81920;

    public StorageManagerService()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string root = Path.Combine(appData, "MCServerManager");

        string serverPath = Path.Combine(root, "server");
        string jarPath = Path.Combine(serverPath, "server.jar");

        RootDirectory = root;
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

    public async Task<string> LoadFileAsStringAsync(string filePath, CancellationToken cancellationToken = default)
    {
        await using FileStream fs = new(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            RandomAccessBufferSize,
            FileOptions.SequentialScan | FileOptions.Asynchronous
        );
        using StreamReader reader = new(fs);

        return await reader.ReadToEndAsync(cancellationToken);
    }

    public async Task SaveFileAsStringAsync(string filePath, string content, CancellationToken cancellationToken = default)
    {
        await using FileStream fs = new(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            RandomAccessBufferSize,
            FileOptions.Asynchronous
        );
        using StreamWriter writer = new(fs);

        await writer.WriteAsync(content.AsMemory(), cancellationToken);

        return;
    }
}

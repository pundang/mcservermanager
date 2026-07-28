using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using MCServerManager.Models;

namespace MCServerManager.Services;

/// <summary>
/// Interface for the service that manages the versions
/// </summary>
public partial class VersionManagerService(IStorageManagerService storageManagerService) : IVersionManagerService, IDisposable
{
    private readonly IStorageManagerService _storageManagerService = storageManagerService;
    private readonly string _manifestUrl = "https://launchermeta.mojang.com/mc/game/version_manifest.json";
    private readonly HttpClient _httpClient = new();

    public Manifest? VersionManifest { get; set; } = null;

    /// <summary>
    /// Fetch the manifest for a list of all the Minecraft versions
    /// </summary>
    public async Task DownloadManifest()
    {
        try
        {
            VersionManifest = await _httpClient.GetFromJsonAsync<Manifest>(_manifestUrl);
        }
        catch (JsonException exc)
        {
            Debug.WriteLine(exc);
        }
    }

    /// <summary>
    /// Downloads the version data using the provided id
    /// </summary>
    private async Task<MinecraftVersion?> DownloadVersionInfo(string versionId)
    {
        if (VersionManifest is null)
        {
            Debug.WriteLine("Getting manifest...");
            await DownloadManifest();
        }

        VersionBase? version = VersionManifest?.Versions.Find(v => v.Id == versionId);

        if (version is null)
            return null;

        var minecraftVersion = await _httpClient.GetFromJsonAsync<MinecraftVersion>(version.Url);

        return minecraftVersion;
    }

    /// <summary>
    /// Downloads the version server binary.
    /// Returns true if able, false if unable
    /// </summary>
    public async Task<bool> DownloadVersionBinary(string versionId)
    {
        MinecraftVersion? minecraftVersion = await DownloadVersionInfo(versionId);

        if (minecraftVersion is null)
            return false;

        DownloadEntry serverDownloadEntry = minecraftVersion.Downloads.Server!;

        if (serverDownloadEntry is null)
            return false; // version exists but doesn't contain download for server

        string url = serverDownloadEntry.Url;
        Stream downloadStream = await _httpClient.GetStreamAsync(url);
        await _storageManagerService.DownloadOrReplaceServerJarAsync(downloadStream);

        return true;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}

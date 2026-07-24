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
public partial class VersionManagerService : IVersionManagerService, IDisposable
{
    private readonly IStorageManagerService _storageManagerService;
    private readonly string _manifestUrl = "https://launchermeta.mojang.com/mc/game/version_manifest.json";
    private readonly HttpClient _httpClient;

    public Manifest? VersionManifest { get; set; }

    public VersionManagerService(IStorageManagerService storageManagerService)
    {
        _storageManagerService = storageManagerService;

        _httpClient = new HttpClient();
        VersionManifest = null;
    }

    /// <summary>
    /// Fetch the manifest for a list of all the Minecraft versions
    /// </summary>
    public async Task GetManifest()
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
    private async Task<MinecraftVersion?> GetManifestVersion(string versionId)
    {
        if (VersionManifest is null)
        {
            Debug.WriteLine("Getting manifest...");
            await GetManifest();
        }

        VersionBase? version = VersionManifest?.Versions.Find(v => v.Id == versionId);

        if (version is null)
            return null;

        var minecraftVersion = await _httpClient.GetFromJsonAsync<MinecraftVersion>(version.Url);

        return minecraftVersion;
    }

    /// <summary>
    /// Downloads the version server binary
    /// </summary>
    public async Task DownloadVersion(string versionId)
    {
        MinecraftVersion? minecraftVersion = await GetManifestVersion(versionId);

        if (minecraftVersion is null)
            return;

        string url = minecraftVersion.Downloads.Server.Url;

        Stream downloadStream = await _httpClient.GetStreamAsync(url);
        await _storageManagerService.DownloadOrReplaceServerJarAsync(downloadStream);

    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}

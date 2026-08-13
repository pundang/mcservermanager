using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using MCServerManager.Models;
using Microsoft.Extensions.Logging;

namespace MCServerManager.Services;

/// <summary>
/// Interface for the service that manages the versions
/// </summary>
public partial class VersionManagerService(
    IStorageManagerService storageManagerService,
    ILogger<VersionManagerService> logger
) : IVersionManagerService, IDisposable
{
    private readonly ILogger<VersionManagerService> _logger = logger;
    private readonly IStorageManagerService _storageManagerService = storageManagerService;
    private readonly string _manifestUrl = "https://launchermeta.mojang.com/mc/game/version_manifest.json";
    private readonly HttpClient _httpClient = new();

    public Manifest? VersionManifest { get; set; } = null;

    /// <summary>
    /// Fetch the manifest for a list of all the Minecraft versions
    /// </summary>
    public async Task<Manifest?> DownloadManifest()
    {
        try
        {
            Manifest? manifest = await _httpClient.GetFromJsonAsync<Manifest>(_manifestUrl);
            VersionManifest = manifest;

            return manifest;
        }
        catch (Exception ex) when (ex is JsonException or HttpRequestException)
        {
            _logger.LogError("Failed to download manifest from URL '{manifestUrl}': {exception}", _manifestUrl, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Downloads the version data using the provided id
    /// </summary>
    private async Task<MinecraftVersion?> DownloadVersionInfo(string versionId)
    {
        if (VersionManifest is null)
        {
            await DownloadManifest();
        }

        VersionBase? version = VersionManifest?.Versions.Find(v => v.Id == versionId);

        if (version is null)
            return null;

        try
        {
            return await _httpClient.GetFromJsonAsync<MinecraftVersion>(version.Url);
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or TaskCanceledException)
        {
            _logger.LogError("Failed to download version info for '{versionId}': {exception}", versionId, ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Downloads the version server binary.
    /// Returns true if able, false if unable
    /// </summary>
    public async Task<DownloadResult> DownloadVersionBinary(string versionId)
    {
        MinecraftVersion? minecraftVersion = await DownloadVersionInfo(versionId);
        if (minecraftVersion is null)
            return DownloadResult.VersionNotFound;

        DownloadEntry? serverDownloadEntry = minecraftVersion.Downloads.Server;
        if (serverDownloadEntry is null)
            return DownloadResult.NoServerJarAvailable;

        try
        {
            await using Stream downloadStream = await _httpClient.GetStreamAsync(serverDownloadEntry.Url);
            await _storageManagerService.DownloadOrReplaceServerJarAsync(downloadStream);
            _logger.LogInformation("Successfully downloaded version '{versionId}'", versionId);
            return DownloadResult.Success;
        }
        catch (Exception ex) when (ex is HttpRequestException or IOException or TaskCanceledException)
        {
            _logger.LogError("Failed to download server jar for '{versionId}': {exception}", versionId, ex.Message);
            return DownloadResult.DownloadFailed;
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}

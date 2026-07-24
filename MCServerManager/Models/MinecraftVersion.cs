using System.Text.Json.Serialization;

namespace MCServerManager.Models;

public class MinecraftVersion : VersionBase
{
    [JsonPropertyName("downloads")]
    public MinecraftVersionDownloads Downloads { get; set; } = new();
}

public class MinecraftVersionDownloads
{
    // I think we won't be using client downloads, i'll leave it here just in case
    [JsonPropertyName("client")]
    public DownloadEntry Client { get; set; } = new();

    [JsonPropertyName("server")]
    public DownloadEntry Server { get; set; } = new();
}

public class DownloadEntry
{
    [JsonPropertyName("sha1")]
    public string Sha1 { get; set; } = string.Empty;

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

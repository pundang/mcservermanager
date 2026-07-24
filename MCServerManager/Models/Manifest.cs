using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MCServerManager.Models;

public class Manifest
{
    [JsonPropertyName("latest")]
    public ManifestLatest Latest { get; set; } = new();

    [JsonPropertyName("versions")]
    public List<VersionBase> Versions { get; set; } = [];
}

public class ManifestLatest
{
    [JsonPropertyName("release")]
    public string Release { get; set; } = string.Empty;

    [JsonPropertyName("snapshot")]
    public string Snapshot { get; set; } = string.Empty;
}

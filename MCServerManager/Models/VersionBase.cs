using System;
using System.Text.Json.Serialization;

namespace MCServerManager.Models;

public class VersionBase
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public VersionBaseType Type { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("time")]
    public DateTimeOffset Time { get; set; }

    [JsonPropertyName("releaseTime")]
    public DateTimeOffset ReleaseTime { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VersionBaseType
{
    [JsonStringEnumMemberName("release")]
    Release,

    [JsonStringEnumMemberName("snapshot")]
    Snapshot,

    [JsonStringEnumMemberName("old_beta")]
    OldBeta,

    [JsonStringEnumMemberName("old_alpha")]
    OldAlpha
}

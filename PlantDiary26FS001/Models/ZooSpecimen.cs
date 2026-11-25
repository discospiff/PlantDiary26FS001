using System.Text.Json.Serialization;

namespace PlantDiary26FS001.Models;

public sealed class ZooSpecimen
{
    [JsonPropertyName("specimen_id")]
    public int SpecimenId { get; init; }
    [JsonPropertyName("plant_id")]
    public int PlantId { get; init; }
    [JsonPropertyName("lat")]
    public double Latitude { get; init; }
    [JsonPropertyName("lng")]
    public double Longitude { get; init; }
    [JsonPropertyName("common")]
    public string? Common { get; init; }
    [JsonPropertyName("genus")]
    public string? Genus { get; init; }
    [JsonPropertyName("species")]
    public string? Species { get; init; }
    [JsonPropertyName("address")]
    public string? Address { get; init; }
    [JsonPropertyName("notes")]
    public string? Notes { get; init; }
}

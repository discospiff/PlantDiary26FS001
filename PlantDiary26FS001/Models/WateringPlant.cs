using System.Text.Json.Serialization;

namespace PlantDiary26FS001.Models;

public sealed class WateringPlant
{
    [JsonPropertyName("id")]
    public int Id { get; init; }
    [JsonPropertyName("genus")]
    public string? Genus { get; init; }
    [JsonPropertyName("species")]
    public string? Species { get; init; }
    [JsonPropertyName("cultivar")]
    public string? Cultivar { get; init; }
    [JsonPropertyName("common")]
    public string? Common { get; init; }
    [JsonPropertyName("wetSoil")]
    public bool? WetSoil { get; init; }
    [JsonPropertyName("drySoil")]
    public bool? DrySoil { get; init; }
    [JsonPropertyName("rainGarden")]
    public bool? RainGarden { get; init; }
    [JsonPropertyName("fullSun")]
    public int? FullSun { get; init; }
    [JsonPropertyName("partShade")]
    public int? PartShade { get; init; }
    [JsonPropertyName("deepShade")]
    public int? DeepShade { get; init; }
}

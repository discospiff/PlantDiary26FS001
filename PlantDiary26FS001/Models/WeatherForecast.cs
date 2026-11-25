using System.Text.Json.Serialization;

namespace PlantDiary26FS001.Models;

public sealed class WeatherForecast
{
    [JsonPropertyName("properties")]
    public WeatherForecastProperties? Properties { get; init; }
}

public sealed class WeatherForecastProperties
{
    [JsonPropertyName("periods")]
    public IReadOnlyList<WeatherForecastPeriod>? Periods { get; init; }
}

using System.Text.Json.Serialization;

namespace PlantDiary26FS001.Models;

public sealed class WeatherForecastPeriod
{
    [JsonPropertyName("probabilityOfPrecipitation")]
    public WeatherProbabilityOfPrecipitation? ProbabilityOfPrecipitation { get; init; }
}

public sealed class WeatherProbabilityOfPrecipitation
{
    [JsonPropertyName("value")]
    public decimal? Value { get; init; }
}

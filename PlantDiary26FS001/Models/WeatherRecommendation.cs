namespace PlantDiary26FS001.Models;

public sealed record WeatherRecommendation(
    bool IsAvailable,
    decimal? ProbabilityOfPrecipitation,
    string? Message)
{
    public static WeatherRecommendation Unavailable() => new(false, null, null);
}

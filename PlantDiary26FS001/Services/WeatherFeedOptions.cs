namespace PlantDiary26FS001.Services;

public sealed class WeatherFeedOptions
{
    public required Uri ForecastUrl { get; init; }
    public TimeSpan CacheDuration { get; init; } = TimeSpan.FromMinutes(5);
    public TimeSpan RequestTimeout { get; init; } = TimeSpan.FromSeconds(10);
}

namespace PlantDiary26FS001.Services;

public sealed class PlantFeedOptions
{
    public required Uri WateringPlantsUrl { get; init; }
    public required Uri SpecimensUrl { get; init; }
    public TimeSpan CacheDuration { get; init; } = TimeSpan.FromMinutes(5);
    public TimeSpan RequestTimeout { get; init; } = TimeSpan.FromSeconds(10);
}

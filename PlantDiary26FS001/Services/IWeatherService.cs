using PlantDiary26FS001.Models;

namespace PlantDiary26FS001.Services;

public interface IWeatherService
{
    Task<WeatherRecommendation> GetRecommendationAsync(CancellationToken cancellationToken = default);
}

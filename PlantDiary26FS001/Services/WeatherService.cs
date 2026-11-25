using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PlantDiary26FS001.Models;

namespace PlantDiary26FS001.Services;

public sealed class WeatherService(
    HttpClient httpClient,
    IOptions<WeatherFeedOptions> options,
    IMemoryCache cache,
    ILogger<WeatherService> logger) : IWeatherService
{
    private const string CacheKey = "plant-diary-weather-recommendation";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly WeatherFeedOptions options = options.Value;

    public async Task<WeatherRecommendation> GetRecommendationAsync(CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(CacheKey, out WeatherRecommendation? cachedRecommendation) && cachedRecommendation is not null)
        {
            return cachedRecommendation;
        }

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(options.RequestTimeout);

        try
        {
            using var response = await httpClient.GetAsync(options.ForecastUrl, timeout.Token);
            response.EnsureSuccessStatusCode();

            var forecast = await response.Content.ReadFromJsonAsync<WeatherForecast>(JsonOptions, timeout.Token);
            var probability = forecast?.Properties?.Periods?.FirstOrDefault()?.ProbabilityOfPrecipitation?.Value;
            if (probability is null or < 0 or > 100)
            {
                return WeatherRecommendation.Unavailable();
            }

            var recommendation = probability > 20
                ? new WeatherRecommendation(true, probability, "No need to water plants")
                : new WeatherRecommendation(true, probability, "Please to water plants");

            cache.Set(CacheKey, recommendation, options.CacheDuration);
            return recommendation;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("The weather feed did not respond within {Timeout}.", options.RequestTimeout);
            return WeatherRecommendation.Unavailable();
        }
        catch (Exception exception) when (exception is HttpRequestException or JsonException)
        {
            logger.LogWarning(exception, "The weather feed could not be interpreted.");
            return WeatherRecommendation.Unavailable();
        }
    }
}

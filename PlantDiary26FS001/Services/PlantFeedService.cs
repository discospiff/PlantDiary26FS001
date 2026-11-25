using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PlantDiary26FS001.Models;

namespace PlantDiary26FS001.Services;

public sealed class PlantFeedService(
    HttpClient httpClient,
    IOptions<PlantFeedOptions> options,
    IMemoryCache cache,
    ILogger<PlantFeedService> logger) : IPlantFeedService
{
    private const string CacheKey = "plant-diary-combined-results";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly PlantFeedOptions options = options.Value;

    public async Task<PlantFeedResult> GetCombinedAsync(CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(CacheKey, out PlantFeedResult? cachedResult) && cachedResult is not null)
        {
            return cachedResult;
        }

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(options.RequestTimeout);

        try
        {
            var wateringPlantsTask = GetFeedAsync<WateringPlant[]>(options.WateringPlantsUrl, timeout.Token);
            var specimensTask = GetFeedAsync<ZooSpecimen[]>(options.SpecimensUrl, timeout.Token);
            await Task.WhenAll(wateringPlantsTask, specimensTask);

            var wateringPlants = wateringPlantsTask.Result;
            var specimens = specimensTask.Result;
            ValidatePlantIdentifiers(wateringPlants);
            ValidateSpecimens(specimens);

            var plantsById = wateringPlants.ToDictionary(plant => plant.Id);
            var results = specimens
                .Where(specimen => plantsById.ContainsKey(specimen.PlantId))
                .Select(specimen => new CombinedSpecimenResult
                {
                    Plant = plantsById[specimen.PlantId],
                    Specimen = specimen
                })
                .OrderBy(result => result.CommonName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(result => result.Specimen.SpecimenId)
                .ToArray();

            var result = results.Length == 0
                ? PlantFeedResult.NoResults()
                : PlantFeedResult.Success(results);

            cache.Set(CacheKey, result, options.CacheDuration);
            return result;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("The plant feeds did not respond within {Timeout}.", options.RequestTimeout);
            return PlantFeedResult.Error("The plant data took too long to respond. Please try again later.");
        }
        catch (Exception exception) when (exception is HttpRequestException or JsonException or InvalidDataException)
        {
            logger.LogWarning(exception, "The plant feeds could not be combined.");
            return PlantFeedResult.Error("The plant data is temporarily unavailable. Please try again later.");
        }
    }

    private async Task<T> GetFeedAsync<T>(Uri uri, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(uri, cancellationToken);
        response.EnsureSuccessStatusCode();

        var value = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
        return value ?? throw new JsonException("The feed response was empty.");
    }

    private static void ValidatePlantIdentifiers(IEnumerable<WateringPlant> plants)
    {
        var values = plants.ToArray();
        if (values.Any(plant => plant.Id <= 0) || values.Select(plant => plant.Id).Distinct().Count() != values.Length)
        {
            throw new InvalidDataException("The watering plant feed contains invalid or duplicate identifiers.");
        }
    }

    private static void ValidateSpecimens(IEnumerable<ZooSpecimen> specimens)
    {
        var values = specimens.ToArray();
        if (values.Any(specimen => specimen.SpecimenId <= 0 || specimen.PlantId <= 0 ||
            specimen.Latitude is < -90 or > 90 || specimen.Longitude is < -180 or > 180))
        {
            throw new InvalidDataException("The specimen feed contains invalid required values.");
        }

        if (values.Select(specimen => specimen.SpecimenId).Distinct().Count() != values.Length)
        {
            throw new InvalidDataException("The specimen feed contains duplicate specimen identifiers.");
        }
    }
}

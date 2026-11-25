using System.Net;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PlantDiary26FS001.Services;

namespace PlantDiary26FS001.Tests.Services;

public sealed class PlantFeedServiceTests
{
    [Fact]
    public async Task GetCombinedAsync_JoinsMatchingSpecimensAndSortsResults()
    {
        var handler = new FeedHandler(
            "[{\"id\":2,\"genus\":\"Acer\",\"species\":\"rubrum\",\"common\":\"Red Maple\"},{\"id\":1,\"genus\":\"Cercis\",\"species\":\"canadensis\",\"common\":\"Eastern Redbud\"}]",
            "[{\"plant_id\":2,\"specimen_id\":20,\"lat\":39.1,\"lng\":-84.5},{\"plant_id\":1,\"specimen_id\":10,\"lat\":39.2,\"lng\":-84.6},{\"plant_id\":99,\"specimen_id\":30,\"lat\":39.3,\"lng\":-84.7}]");
        var service = CreateService(handler);

        var result = await service.GetCombinedAsync();

        Assert.Equal(PlantFeedStatus.Success, result.Status);
        Assert.Collection(result.Results,
            item => Assert.Equal(10, item.Specimen.SpecimenId),
            item => Assert.Equal(20, item.Specimen.SpecimenId));
        Assert.Equal(2, handler.RequestCount);
    }

    [Fact]
    public async Task GetCombinedAsync_ReturnsErrorForDuplicateIdentifiers()
    {
        var handler = new FeedHandler(
            "[{\"id\":1},{\"id\":1}]",
            "[]");
        var service = CreateService(handler);

        var result = await service.GetCombinedAsync();

        Assert.Equal(PlantFeedStatus.Error, result.Status);
        Assert.Empty(result.Results);
    }

    [Fact]
    public async Task GetCombinedAsync_ReturnsErrorForMalformedJson()
    {
        var handler = new FeedHandler("not-json", "[]");
        var service = CreateService(handler);

        var result = await service.GetCombinedAsync();

        Assert.Equal(PlantFeedStatus.Error, result.Status);
        Assert.Empty(result.Results);
    }

    [Fact]
    public async Task GetCombinedAsync_ReturnsErrorForInvalidCoordinates()
    {
        var handler = new FeedHandler(
            "[{\"id\":1}]",
            "[{\"plant_id\":1,\"specimen_id\":1,\"lat\":91,\"lng\":-84.5}]");
        var service = CreateService(handler);

        var result = await service.GetCombinedAsync();

        Assert.Equal(PlantFeedStatus.Error, result.Status);
        Assert.Empty(result.Results);
    }

    [Fact]
    public async Task GetCombinedAsync_ReturnsErrorWhenEitherFeedFails()
    {
        var handler = new FeedHandler(
            "[{\"id\":1}]",
            "[]") { FailSpecimens = true };
        var service = CreateService(handler);

        var result = await service.GetCombinedAsync();

        Assert.Equal(PlantFeedStatus.Error, result.Status);
        Assert.Empty(result.Results);
    }

    [Fact]
    public async Task GetCombinedAsync_ReturnsNoResultsForSuccessfulUnmatchedFeeds()
    {
        var handler = new FeedHandler(
            "[{\"id\":1}]",
            "[{\"plant_id\":2,\"specimen_id\":1,\"lat\":39.1,\"lng\":-84.5}]");
        var service = CreateService(handler);

        var result = await service.GetCombinedAsync();

        Assert.Equal(PlantFeedStatus.NoResults, result.Status);
        Assert.Empty(result.Results);
    }

    [Fact]
    public async Task GetCombinedAsync_CachesSuccessfulResult()
    {
        var handler = new FeedHandler(
            "[{\"id\":1,\"common\":\"Redbud\"}]",
            "[{\"plant_id\":1,\"specimen_id\":1,\"lat\":39.1,\"lng\":-84.5}]");
        var service = CreateService(handler);

        await service.GetCombinedAsync();
        await service.GetCombinedAsync();

        Assert.Equal(2, handler.RequestCount);
    }

    private static PlantFeedService CreateService(FeedHandler handler)
    {
        var options = Options.Create(new PlantFeedOptions
        {
            WateringPlantsUrl = new Uri("https://example.test/plants.json"),
            SpecimensUrl = new Uri("https://example.test/specimens.json"),
            CacheDuration = TimeSpan.FromMinutes(5),
            RequestTimeout = TimeSpan.FromSeconds(5)
        });
        return new PlantFeedService(
            new HttpClient(handler),
            options,
            new MemoryCache(new MemoryCacheOptions()),
            NullLogger<PlantFeedService>.Instance);
    }

    private sealed class FeedHandler(string plantsJson, string specimensJson) : HttpMessageHandler
    {
        public bool FailSpecimens { get; set; }
        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            if (FailSpecimens && request.RequestUri?.AbsolutePath.Contains("specimens", StringComparison.OrdinalIgnoreCase) == true)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
            }

            var json = request.RequestUri?.AbsolutePath.Contains("plants", StringComparison.OrdinalIgnoreCase) == true
                ? plantsJson
                : specimensJson;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
        }
    }
}

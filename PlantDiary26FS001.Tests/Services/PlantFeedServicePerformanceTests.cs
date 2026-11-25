using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PlantDiary26FS001.Services;

namespace PlantDiary26FS001.Tests.Services;

public sealed class PlantFeedServicePerformanceTests
{
    [Fact]
    public async Task CachedResult_AvoidsRepeatedFeedRequests()
    {
        var handler = new CountingHandler();
        var service = new PlantFeedService(
            new HttpClient(handler),
            Options.Create(new PlantFeedOptions
            {
                WateringPlantsUrl = new Uri("https://example.test/plants.json"),
                SpecimensUrl = new Uri("https://example.test/specimens.json"),
                CacheDuration = TimeSpan.FromMinutes(5),
                RequestTimeout = TimeSpan.FromSeconds(5)
            }),
            new MemoryCache(new MemoryCacheOptions()),
            NullLogger<PlantFeedService>.Instance);

        await service.GetCombinedAsync();
        await service.GetCombinedAsync();

        Assert.Equal(2, handler.RequestCount);
    }

    private sealed class CountingHandler : HttpMessageHandler
    {
        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            var json = request.RequestUri?.AbsolutePath.Contains("plants", StringComparison.OrdinalIgnoreCase) == true
                ? "[]"
                : "[]";
            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            });
        }
    }
}

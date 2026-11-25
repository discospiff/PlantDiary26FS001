using System.Net;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PlantDiary26FS001.Models;
using PlantDiary26FS001.Services;

namespace PlantDiary26FS001.Tests.Services;

public sealed class WeatherServiceTests
{
    [Theory]
    [InlineData(21, "No need to water plants")]
    [InlineData(20, "Please to water plants")]
    [InlineData(0, "Please to water plants")]
    public async Task GetRecommendationAsync_MapsThreshold(decimal probability, string expectedMessage)
    {
        var handler = new WeatherHandler($"{{\"properties\":{{\"periods\":[{{\"probabilityOfPrecipitation\":{{\"value\":{probability}}}}}]}}}}");
        var service = CreateService(handler);

        var result = await service.GetRecommendationAsync();

        Assert.True(result.IsAvailable);
        Assert.Equal(probability, result.ProbabilityOfPrecipitation);
        Assert.Equal(expectedMessage, result.Message);
    }

    [Fact]
    public async Task GetRecommendationAsync_UsesFirstPeriod()
    {
        var handler = new WeatherHandler("{\"properties\":{\"periods\":[{\"probabilityOfPrecipitation\":{\"value\":20}},{\"probabilityOfPrecipitation\":{\"value\":80}}]}}");
        var service = CreateService(handler);

        var result = await service.GetRecommendationAsync();

        Assert.Equal(20, result.ProbabilityOfPrecipitation);
        Assert.Equal("Please to water plants", result.Message);
    }

    [Theory]
    [InlineData("{\"properties\":{\"periods\":[]}}")]
    [InlineData("{\"properties\":{\"periods\":[{\"probabilityOfPrecipitation\":{\"value\":null}}]}}")]
    [InlineData("{\"properties\":{\"periods\":[{\"probabilityOfPrecipitation\":{\"value\":101}}]}}")]
    [InlineData("not-json")]
    public async Task GetRecommendationAsync_ReturnsUnavailableForUnusableData(string responseJson)
    {
        var service = CreateService(new WeatherHandler(responseJson));

        var result = await service.GetRecommendationAsync();

        Assert.False(result.IsAvailable);
        Assert.Null(result.Message);
    }

    [Fact]
    public async Task GetRecommendationAsync_ReturnsUnavailableForHttpFailure()
    {
        var service = CreateService(new WeatherHandler("{}") { StatusCode = HttpStatusCode.ServiceUnavailable });

        var result = await service.GetRecommendationAsync();

        Assert.False(result.IsAvailable);
    }

    [Fact]
    public async Task GetRecommendationAsync_ReturnsUnavailableWhenRequestTimesOut()
    {
        var handler = new WeatherHandler("{}") { Delay = TimeSpan.FromMilliseconds(100) };
        var options = Options.Create(new WeatherFeedOptions
        {
            ForecastUrl = new Uri("https://example.test/forecast"),
            CacheDuration = TimeSpan.FromMinutes(5),
            RequestTimeout = TimeSpan.FromMilliseconds(1)
        });
        var service = new WeatherService(
            new HttpClient(handler),
            options,
            new MemoryCache(new MemoryCacheOptions()),
            NullLogger<WeatherService>.Instance);

        var result = await service.GetRecommendationAsync();

        Assert.False(result.IsAvailable);
    }

    [Fact]
    public async Task GetRecommendationAsync_CachesSuccessfulResult()
    {
        var handler = new WeatherHandler("{\"properties\":{\"periods\":[{\"probabilityOfPrecipitation\":{\"value\":30}}]}}");
        var service = CreateService(handler);

        await service.GetRecommendationAsync();
        await service.GetRecommendationAsync();

        Assert.Equal(1, handler.RequestCount);
    }

    private static WeatherService CreateService(WeatherHandler handler)
    {
        var options = Options.Create(new WeatherFeedOptions
        {
            ForecastUrl = new Uri("https://example.test/forecast"),
            CacheDuration = TimeSpan.FromMinutes(5),
            RequestTimeout = TimeSpan.FromSeconds(5)
        });

        return new WeatherService(
            new HttpClient(handler),
            options,
            new MemoryCache(new MemoryCacheOptions()),
            NullLogger<WeatherService>.Instance);
    }

    private sealed class WeatherHandler(string responseJson) : HttpMessageHandler
    {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public TimeSpan Delay { get; set; }
        public int RequestCount { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            if (Delay > TimeSpan.Zero)
            {
                await Task.Delay(Delay, cancellationToken);
            }

            return new HttpResponseMessage(StatusCode)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };
        }
    }
}

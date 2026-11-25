using PlantDiary26FS001.Models;
using PlantDiary26FS001.Pages;
using PlantDiary26FS001.Services;

namespace PlantDiary26FS001.Tests.Pages;

public sealed class IndexPageModelTests
{
    [Fact]
    public async Task OnGetAsync_ExposesCombinedResults()
    {
        var model = new IndexModel(new FakePlantFeedService(PlantFeedResult.Success(
            [new CombinedSpecimenResult
            {
                Plant = new WateringPlant { Id = 1, Common = "Redbud", Genus = "Cercis", Species = "canadensis" },
                Specimen = new ZooSpecimen { PlantId = 1, SpecimenId = 10, Latitude = 39.1, Longitude = -84.5 }
            }])), new FakeWeatherService(new WeatherRecommendation(true, 30, "No need to water plants")));

        await model.OnGetAsync(CancellationToken.None);

        Assert.Equal(PlantPageStatus.Success, model.Status);
        Assert.Single(model.Results);
        Assert.Equal("Redbud", model.Results[0].CommonName);
        Assert.Equal("No need to water plants", model.WeatherRecommendation?.Message);
    }

    [Fact]
    public async Task OnGetAsync_ExposesNoResultsState()
    {
        var model = new IndexModel(new FakePlantFeedService(PlantFeedResult.NoResults()), new FakeWeatherService(WeatherRecommendation.Unavailable()));

        await model.OnGetAsync(CancellationToken.None);

        Assert.Equal(PlantPageStatus.NoResults, model.Status);
        Assert.Empty(model.Results);
    }

    [Fact]
    public async Task OnGetAsync_ExposesErrorStateWithoutResults()
    {
        var model = new IndexModel(new FakePlantFeedService(PlantFeedResult.Error("Feed unavailable.")), new FakeWeatherService(WeatherRecommendation.Unavailable()));

        await model.OnGetAsync(CancellationToken.None);

        Assert.Equal(PlantPageStatus.Error, model.Status);
        Assert.Empty(model.Results);
        Assert.Equal("Feed unavailable.", model.ErrorMessage);
    }

    [Fact]
    public async Task OnGetAsync_PreservesPlantResultsWhenWeatherIsUnavailable()
    {
        var plant = new CombinedSpecimenResult
        {
            Plant = new WateringPlant { Id = 1, Common = "Redbud", Genus = "Cercis", Species = "canadensis" },
            Specimen = new ZooSpecimen { PlantId = 1, SpecimenId = 10, Latitude = 39.1, Longitude = -84.5 }
        };
        var model = new IndexModel(
            new FakePlantFeedService(PlantFeedResult.Success([plant])),
            new FakeWeatherService(WeatherRecommendation.Unavailable()));

        await model.OnGetAsync(CancellationToken.None);

        Assert.Equal(PlantPageStatus.Success, model.Status);
        Assert.Single(model.Results);
        Assert.False(model.WeatherRecommendation?.IsAvailable);
    }

    [Theory]
    [InlineData(true, "Yes")]
    [InlineData(false, "No")]
    [InlineData(null, "Not provided")]
    public void FormatAvailability_UsesClearLabels(bool? value, string expected)
    {
        Assert.Equal(expected, IndexModel.FormatAvailability(value));
    }

    private sealed class FakePlantFeedService(PlantFeedResult result) : IPlantFeedService
    {
        public Task<PlantFeedResult> GetCombinedAsync(CancellationToken cancellationToken = default) => Task.FromResult(result);
    }

    private sealed class FakeWeatherService(WeatherRecommendation result) : IWeatherService
    {
        public Task<WeatherRecommendation> GetRecommendationAsync(CancellationToken cancellationToken = default) => Task.FromResult(result);
    }
}

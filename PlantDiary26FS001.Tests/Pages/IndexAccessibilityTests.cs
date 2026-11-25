namespace PlantDiary26FS001.Tests.Pages;

public sealed class IndexAccessibilityTests
{
    [Fact]
    public void AccessibilityRequirements_AreDocumentedForPageReview()
    {
        Assert.True(File.Exists(FindRepositoryFile("PlantDiary26FS001/Pages/Index.cshtml")));
        Assert.True(File.Exists(FindRepositoryFile("PlantDiary26FS001/wwwroot/css/site.css")));
    }

    [Fact]
    public void WeatherRecommendation_HasAccessibleContextAndConditionalRendering()
    {
        var page = File.ReadAllText(FindRepositoryFile("PlantDiary26FS001/Pages/Index.cshtml"));

        Assert.Contains("weather-recommendation", page, StringComparison.Ordinal);
        Assert.Contains("aria-labelledby=\"weather-heading\"", page, StringComparison.Ordinal);
        Assert.Contains("Cincinnati Zoo weather guidance", page, StringComparison.Ordinal);
        Assert.Contains("Model.WeatherRecommendation?.IsAvailable == true", page, StringComparison.Ordinal);
    }

    private static string FindRepositoryFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "PlantDiary26FS001.slnx")))
        {
            directory = directory.Parent;
        }

        return Path.Combine(directory?.FullName ?? AppContext.BaseDirectory, relativePath);
    }
}

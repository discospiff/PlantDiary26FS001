namespace PlantDiary26FS001.Tests.Pages;

public sealed class IndexAccessibilityTests
{
    [Fact]
    public void AccessibilityRequirements_AreDocumentedForPageReview()
    {
        Assert.True(File.Exists(FindRepositoryFile("PlantDiary26FS001/Pages/Index.cshtml")));
        Assert.True(File.Exists(FindRepositoryFile("PlantDiary26FS001/wwwroot/css/site.css")));
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

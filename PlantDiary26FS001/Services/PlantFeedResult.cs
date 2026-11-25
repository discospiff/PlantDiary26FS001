using PlantDiary26FS001.Models;

namespace PlantDiary26FS001.Services;

public enum PlantFeedStatus
{
    Success,
    NoResults,
    Error
}

public sealed class PlantFeedResult
{
    private PlantFeedResult(PlantFeedStatus status, IReadOnlyList<CombinedSpecimenResult> results, string? errorMessage)
    {
        Status = status;
        Results = results;
        ErrorMessage = errorMessage;
    }

    public PlantFeedStatus Status { get; }
    public IReadOnlyList<CombinedSpecimenResult> Results { get; }
    public string? ErrorMessage { get; }

    public static PlantFeedResult Success(IReadOnlyList<CombinedSpecimenResult> results) => new(PlantFeedStatus.Success, results, null);
    public static PlantFeedResult NoResults() => new(PlantFeedStatus.NoResults, Array.Empty<CombinedSpecimenResult>(), null);
    public static PlantFeedResult Error(string message) => new(PlantFeedStatus.Error, Array.Empty<CombinedSpecimenResult>(), message);
}

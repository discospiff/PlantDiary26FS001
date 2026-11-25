namespace PlantDiary26FS001.Services;

public interface IPlantFeedService
{
    Task<PlantFeedResult> GetCombinedAsync(CancellationToken cancellationToken = default);
}

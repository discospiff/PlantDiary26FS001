using Microsoft.AspNetCore.Mvc.RazorPages;
using PlantDiary26FS001.Models;
using PlantDiary26FS001.Services;

namespace PlantDiary26FS001.Pages
{
    public class IndexModel(IPlantFeedService plantFeedService) : PageModel
    {
        public IReadOnlyList<CombinedSpecimenResult> Results { get; private set; } = Array.Empty<CombinedSpecimenResult>();
        public PlantPageStatus Status { get; private set; } = PlantPageStatus.Loading;
        public string? ErrorMessage { get; private set; }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            var result = await plantFeedService.GetCombinedAsync(cancellationToken);
            Results = result.Results;
            ErrorMessage = result.ErrorMessage;
            Status = result.Status switch
            {
                PlantFeedStatus.Success => PlantPageStatus.Success,
                PlantFeedStatus.NoResults => PlantPageStatus.NoResults,
                _ => PlantPageStatus.Error
            };
        }

        public static string FormatAvailability(bool? value) => value switch
        {
            true => "Yes",
            false => "No",
            _ => "Not provided"
        };
    }

    public enum PlantPageStatus
    {
        Loading,
        Success,
        NoResults,
        Error
    }
}

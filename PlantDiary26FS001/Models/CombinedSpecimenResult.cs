namespace PlantDiary26FS001.Models;

public sealed class CombinedSpecimenResult
{
    public required WateringPlant Plant { get; init; }
    public required ZooSpecimen Specimen { get; init; }

    public string CommonName => Plant.Common ?? Specimen.Common ?? "Unnamed plant";
    public string ScientificName => string.Join(" ", new[] { Plant.Genus ?? Specimen.Genus, Plant.Species ?? Specimen.Species }.Where(value => !string.IsNullOrWhiteSpace(value)));
}

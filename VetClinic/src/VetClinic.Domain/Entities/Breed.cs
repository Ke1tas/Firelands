using VetClinic.Domain.Enums;

namespace VetClinic.Domain.Entities;

public class Breed{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AnimalSpecies Species { get; set; }
}
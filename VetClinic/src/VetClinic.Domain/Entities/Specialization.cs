using VetClinic.Domain.Enums;

namespace VetClinic.Domain.Entities;

public class Specialization
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AnimalSpecies TargetSpecies { get; set; }
}
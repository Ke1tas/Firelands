using VetClinic.Domain.Enums;

namespace VetClinic.Domain.Enteties;

public class Pet{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AnimalSpecies Species { get; set; }
    
    public int BreedId { get; set; }
    public Breed Breed { get; set; } = null!;

    public DateOnly BirthDate { get; set; }
    public decimal WeightKg { get; set; }

    public int OwnerId { get; set; }
    public Owner Owner { get; set; } = null!;

    public List<Appointment> Appointments { get; set; } = [];
}
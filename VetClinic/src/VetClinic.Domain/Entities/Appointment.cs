namespace VetClinic.Domain.Entities;

public class Appointment{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public int RoomNumber { get; set; }
    public bool IsFollowUp { get; set; }

    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;

    public int VeterinarianId { get; set; }
    public Veterinarian Veterinarian { get; set; } = null!;
}
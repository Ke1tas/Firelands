namespace VetClinic.Domain.Enteties;

public class Veterinarian{
    public int Id { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int BirthYear { get; set; }

    public int SpecializationId { get; set; }
    public Specialization Specialization { get; set; } = null!;

    public int ExperienceYears { get; set; }

    public List<Appointment> Appointments { get; set; } = [];
}
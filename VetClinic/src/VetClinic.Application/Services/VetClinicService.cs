using VetClinic.Domain.Entities;
using VetClinic.Domain.Enums;

namespace VetClinic.Application.Services;

public interface IVetClinicService
{
    public IEnumerable<Veterinarian> GetVeterinariansBySpecies(AnimalSpecies species);
    public IEnumerable<Pet> GetPetsByDoctorOrderedByName(int doctorId);
    public int GetFollowUpAppointmentsCountByBreed(int breedId);
    public IEnumerable<Owner> GetOwnersWithMultiplePetsOrderedByName();
    public IEnumerable<Appointment> GetCurrentMonthAppointmentsByRoom(int roomNumber, DateTime currentDate);
}

public class VetClinicService(
    IEnumerable<Veterinarian> veterinarians,
    IEnumerable<Appointment> appointments,
    IEnumerable<Owner> owners) : IVetClinicService
{
    public IEnumerable<Veterinarian> GetVeterinariansBySpecies(AnimalSpecies species)
    {
        return veterinarians
            .Where(v => v.Specialization.TargetSpecies == species);
    }

    public IEnumerable<Pet> GetPetsByDoctorOrderedByName(int doctorId)
    {
        return appointments
            .Where(a => a.VeterinarianId == doctorId)
            .Select(a => a.Pet)
            .DistinctBy(p => p.Id)
            .OrderBy(p => p.Name);
    }

    public int GetFollowUpAppointmentsCountByBreed(int breedId)
    {
        return appointments
            .Count(a => a.Pet.BreedId == breedId && a.IsFollowUp);
    }

    public IEnumerable<Owner> GetOwnersWithMultiplePetsOrderedByName()
    {
        return owners
            .Where(o => o.Pets.Count > 1)
            .OrderBy(o => o.FullName);
    }

    public IEnumerable<Appointment> GetCurrentMonthAppointmentsByRoom(int roomNumber, DateTime currentDate)
    {
        return appointments
            .Where(a => a.RoomNumber == roomNumber 
                        && a.DateTime.Year == currentDate.Year 
                        && a.DateTime.Month == currentDate.Month);
    }
}
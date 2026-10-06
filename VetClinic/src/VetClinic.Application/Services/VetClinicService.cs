using VetClinic.Domain.Entities;
using VetClinic.Domain.Enums;
using VetClinic.Domain.Interfaces;

namespace VetClinic.Application.Services;

/// <summary>
/// Реализация сервиса аналитических запросов к данным ветеринарной клиники
/// </summary>
public class VetClinicService(
    IEnumerable<Veterinarian> veterinarians,
    IEnumerable<Appointment> appointments,
    IEnumerable<Owner> owners) : IVetClinicService
{
    /// <inheritdoc />
    public IEnumerable<Veterinarian> GetVeterinariansBySpecies(AnimalSpecies species)
    {
        return veterinarians
            .Where(v => v.Specialization?.TargetSpecies == species);
    }

    /// <inheritdoc />
    public IEnumerable<Pet> GetPetsByDoctorOrderedByName(int doctorId)
    {
        return appointments
            .Where(a => a.VeterinarianId == doctorId && a.Pet is not null)
            .Select(a => a.Pet!)
            .DistinctBy(p => p.Id)
            .OrderBy(p => p.Name);
    }

    /// <inheritdoc />
    public int GetFollowUpAppointmentsCountByBreed(int breedId)
    {
        return appointments
            .Count(a => a.Pet?.BreedId == breedId && a.IsFollowUp);
    }

    /// <inheritdoc />
    public IEnumerable<Owner> GetOwnersWithMultiplePetsOrderedByName()
    {
        return owners
            .Where(o => o.Pets.Count > 1)
            .OrderBy(o => o.FullName);
    }

    /// <inheritdoc />
    public IEnumerable<Appointment> GetCurrentMonthAppointmentsByRoom(int roomNumber, DateTime currentDate)
    {
        return appointments
            .Where(a => a.RoomNumber == roomNumber
                        && a.DateTime.Year == currentDate.Year
                        && a.DateTime.Month == currentDate.Month);
    }
}
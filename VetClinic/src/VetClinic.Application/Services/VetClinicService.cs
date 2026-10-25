using VetClinic.Domain.Entities;
using VetClinic.Domain.Enums;
using VetClinic.Domain.Interfaces;

namespace VetClinic.Application.Services;

/// <summary>
/// Реализация сервиса аналитических запросов к данным ветеринарной клиники
/// </summary>
public class VetClinicService : IVetClinicService
{
    private readonly Func<IEnumerable<Veterinarian>> _getVeterinarians;
    private readonly Func<IEnumerable<Appointment>> _getAppointments;
    private readonly Func<IEnumerable<Owner>> _getOwners;

    /// <summary>
    /// Конструктор для использования через Dependency Injection с живыми репозиториями
    /// </summary>
    public VetClinicService(
        IRepository<Veterinarian> vetRepo,
        IRepository<Appointment> appRepo,
        IRepository<Owner> ownerRepo)
    {
        _getVeterinarians = vetRepo.GetAll;
        _getAppointments = appRepo.GetAll;
        _getOwners = ownerRepo.GetAll;
    }

    /// <summary>
    /// Конструктор для юнит-тестов первой лабораторной работы
    /// </summary>
    public VetClinicService(
        IEnumerable<Veterinarian> veterinarians,
        IEnumerable<Appointment> appointments,
        IEnumerable<Owner> owners)
    {
        _getVeterinarians = () => veterinarians;
        _getAppointments = () => appointments;
        _getOwners = () => owners;
    }

    /// <inheritdoc />
    public IEnumerable<Veterinarian> GetVeterinariansBySpecies(AnimalSpecies species)
    {
        return _getVeterinarians()
            .Where(v => v.Specialization?.TargetSpecies == species);
    }

    /// <inheritdoc />
    public IEnumerable<Pet> GetPetsByDoctorOrderedByName(int doctorId)
    {
        return _getAppointments()
            .Where(a => a.VeterinarianId == doctorId && a.Pet is not null)
            .Select(a => a.Pet!)
            .DistinctBy(p => p.Id)
            .OrderBy(p => p.Name);
    }

    /// <inheritdoc />
    public int GetFollowUpAppointmentsCountByBreed(int breedId)
    {
        return _getAppointments()
            .Count(a => a.Pet?.BreedId == breedId && a.IsFollowUp);
    }

    /// <inheritdoc />
    public IEnumerable<Owner> GetOwnersWithMultiplePetsOrderedByName()
    {
        return _getOwners()
            .Where(o => o.Pets.Count > 1)
            .OrderBy(o => o.FullName);
    }

    /// <inheritdoc />
    public IEnumerable<Appointment> GetCurrentMonthAppointmentsByRoom(int roomNumber, DateTime currentDate)
    {
        return _getAppointments()
            .Where(a => a.RoomNumber == roomNumber
                        && a.DateTime.Year == currentDate.Year
                        && a.DateTime.Month == currentDate.Month);
    }
}
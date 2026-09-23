using VetClinic.Application.Services;
using VetClinic.Domain.Enums;
using VetClinic.Infrastructure.Seeding;

namespace VetClinic.UnitTests;

public class VetClinicServiceTests
{
    private readonly ClinicDataSeeder _seeder;
    private readonly VetClinicService _service;

    public VetClinicServiceTests()
    {
        _seeder = new ClinicDataSeeder(seed: 67148867);
        _service = new VetClinicService(
            _seeder.Veterinarians,
            _seeder.Appointments,
            _seeder.Owners
        );
    }

    [Fact]
    public void GetVeterinariansBySpecies_ReturnsOnlyVetsWithMatchingSpecialization()
    {
        const AnimalSpecies targetSpecies = AnimalSpecies.Cat;

        var result = _service.GetVeterinariansBySpecies(targetSpecies).ToList();

        Assert.NotEmpty(result);
        Assert.All(result, v => Assert.Equal(targetSpecies, v.Specialization.TargetSpecies));
    }

    [Fact]
    public void GetPetsByDoctorOrderedByName_ReturnsPetsOrderedAlphabetically()
    {
        var doctorWithAppointments = _seeder.Veterinarians
            .First(v => v.Appointments.Count > 1);

        var result = _service.GetPetsByDoctorOrderedByName(doctorWithAppointments.Id).ToList();

        Assert.NotEmpty(result);
        var expectedOrderedNames = result.Select(p => p.Name).OrderBy(n => n).ToList();
        Assert.Equal(expectedOrderedNames, result.Select(p => p.Name));
    }

    [Fact]
    public void GetFollowUpAppointmentsCountByBreed_ReturnsCorrectCount()
    {
        var breed = _seeder.Breeds.First();
        var expectedCount = _seeder.Appointments
            .Count(a => a.Pet.BreedId == breed.Id && a.IsFollowUp);

        var actualCount = _service.GetFollowUpAppointmentsCountByBreed(breed.Id);

        Assert.Equal(expectedCount, actualCount);
    }

    [Fact]
    public void GetOwnersWithMultiplePetsOrderedByName_ReturnsFilteredAndSortedOwners()
    {
        var result = _service.GetOwnersWithMultiplePetsOrderedByName().ToList();

        Assert.NotEmpty(result);
        Assert.All(result, o => Assert.True(o.Pets.Count > 1));

        var names = result.Select(o => o.FullName).ToList();
        var sortedNames = names.OrderBy(n => n).ToList();
        Assert.Equal(sortedNames, names);
    }

    [Fact]
    public void GetCurrentMonthAppointmentsByRoom_ReturnsOnlyAppointmentsForCurrentMonthAndRoom()
    {
        var sampleAppointment = _seeder.Appointments.First();
        var date = sampleAppointment.DateTime;
        var room = sampleAppointment.RoomNumber;

        var result = _service.GetCurrentMonthAppointmentsByRoom(room, date).ToList();

        Assert.All(result, a =>
        {
            Assert.Equal(room, a.RoomNumber);
            Assert.Equal(date.Year, a.DateTime.Year);
            Assert.Equal(date.Month, a.DateTime.Month);
        });
    }
}
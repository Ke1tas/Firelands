using VetClinic.Application.Services;
using VetClinic.Domain.Enums;
using VetClinic.Domain.Interfaces;
using VetClinic.Infrastructure.Seeding;
using Xunit;

namespace VetClinic.UnitTests;

/// <summary>
/// Юнит-тесты для аналитических запросов ветеринарной клиники
/// </summary>
public class VetClinicServiceTests
{
    private readonly IVetClinicService _service;

    /// <summary>
    /// Инициализация сервиса с фиксированными тестовыми данными.
    /// </summary>
    public VetClinicServiceTests()
    {
        var seeder = new ClinicDataSeeder();
        _service = new VetClinicService(
            seeder.Veterinarians,
            seeder.Appointments,
            seeder.Owners
        );
    }

    /// <summary>
    /// Проверка: Вывод всех ветеринаров, специализирующихся на кошках
    /// Ожидаем ровно 3 конкретных врачей
    /// </summary>
    [Fact]
    public void GetVeterinariansBySpecies_ReturnsExactVeterinariansForCatSpecies()
    {
        // Arrange
        const AnimalSpecies targetSpecies = AnimalSpecies.Cat;
        const int expectedCount = 3;
        var expectedVeterinarianNames = new HashSet<string>
        {
            "Смирнов Андрей Викторович",
            "Кузнецова Анна Павловна",
            "Морозов Игорь Сергеевич"
        };

        // Act
        var result = _service.GetVeterinariansBySpecies(targetSpecies).ToList();

        // Assert
        Assert.Equal(expectedCount, result.Count);
        var actualNames = result.Select(v => v.FullName).ToHashSet();
        Assert.Equal(expectedVeterinarianNames, actualNames);
        Assert.All(result, v =>
            Assert.Equal(targetSpecies, v.Specialization?.TargetSpecies));
    }

    /// <summary>
    /// Проверка: Вывод всех питомцев, записанных к врачу Поповой - Id = 4 - упорядоченных по кличке
    /// Записаны питомцы: Рекс и Шарик. Ожидаем 2 питомца в алфавитном порядке
    /// </summary>
    [Fact]
    public void GetPetsByDoctorOrderedByName_ReturnsUniquePetsSortedAlphabetically()
    {
        // Arrange
        const int doctorId = 4;
        const int expectedCount = 2;
        var expectedPetNames = new List<string> { "Рекс", "Шарик" };

        // Act
        var result = _service.GetPetsByDoctorOrderedByName(doctorId).ToList();

        // Assert
        Assert.Equal(expectedCount, result.Count);
        var actualPetNames = result.Select(p => p.Name).ToList();
        Assert.Equal(expectedPetNames, actualPetNames);
    }

    /// <summary>
    /// Проверка: Количество повторных приемов для породы Лабрадор ретривер - BreedId = 1
    /// Из 4 приемов для лабрадора повторными являются ровно 2 приема - Appointments 2 и 8
    /// </summary>
    [Fact]
    public void GetFollowUpAppointmentsCountByBreed_ReturnsExactCountOfFollowUps()
    {
        // Arrange
        const int labradorBreedId = 1;
        const int expectedFollowUpCount = 2;

        // Act
        var actualCount = _service.GetFollowUpAppointmentsCountByBreed(labradorBreedId);

        // Assert
        Assert.Equal(expectedFollowUpCount, actualCount);
    }

    /// <summary>
    /// Проверка: Вывод владельцев, имеющих более одного питомца, упорядоченных по ФИО
    /// У Иванова (2 питомца) и Петрова (2 питомца)
    /// Ожидаем ровно 2 владельца в алфавитном порядке
    /// </summary>
    [Fact]
    public void GetOwnersWithMultiplePetsOrderedByName_ReturnsFilteredAndAlphabeticallySortedOwners()
    {
        // Arrange
        const int expectedCount = 2;
        var expectedOwnerNames = new List<string>
        {
            "Иванов Иван Иванович",
            "Петров Петр Петрович"
        };

        // Act
        var result = _service.GetOwnersWithMultiplePetsOrderedByName().ToList();

        // Assert
        Assert.Equal(expectedCount, result.Count);
        var actualOwnerNames = result.Select(o => o.FullName).ToList();
        Assert.Equal(expectedOwnerNames, actualOwnerNames);
        Assert.All(result, o => Assert.True(o.Pets.Count > 1));
    }

    /// <summary>
    /// Проверка: Вывод приемов за сентябрь 2026 года в кабинете 101
    /// В кабинете 101 было 4 приема: 3 в сентябре и 1 в августе
    /// Ожидаем 3 приема, относящихся к сентябрю
    /// </summary>
    [Fact]
    public void GetCurrentMonthAppointmentsByRoom_ReturnsOnlyAppointmentsForSpecifiedMonthAndRoom()
    {
        // Arrange
        const int targetRoom = 101;
        var testDate = new DateTime(2026, 9, 1);
        const int expectedCount = 3;
        var expectedAppointmentIds = new HashSet<int> { 1, 2, 3 };

        // Act
        var result = _service.GetCurrentMonthAppointmentsByRoom(targetRoom, testDate).ToList();

        // Assert
        Assert.Equal(expectedCount, result.Count);
        var actualAppointmentIds = result.Select(a => a.Id).ToHashSet();
        Assert.Equal(expectedAppointmentIds, actualAppointmentIds);
        Assert.All(result, a =>
        {
            Assert.Equal(targetRoom, a.RoomNumber);
            Assert.Equal(testDate.Year, a.DateTime.Year);
            Assert.Equal(testDate.Month, a.DateTime.Month);
        });
    }
}
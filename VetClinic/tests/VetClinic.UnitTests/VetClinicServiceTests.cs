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
    /// Инициализация сервиса с фиксированными тестовыми данными
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
        var result = _service.GetVeterinariansBySpecies(AnimalSpecies.Cat).ToList();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, v => v.FullName == "Смирнов Андрей Викторович");
        Assert.Contains(result, v => v.FullName == "Кузнецова Анна Павловна");
        Assert.Contains(result, v => v.FullName == "Морозов Игорь Сергеевич");
        Assert.All(result, v => Assert.Equal(AnimalSpecies.Cat, v.Specialization?.TargetSpecies));
    }

    /// <summary>
    /// Проверка: Вывод всех питомцев, записанных к врачу Поповой - Id = 4 - упорядоченных по кличке
    /// Записаны питомцы: Рекс и Шарик. Ожидаем 2 питомца в алфавитном порядке
    /// </summary>
    [Fact]
    public void GetPetsByDoctorOrderedByName_ReturnsUniquePetsSortedAlphabetically()
    {
        const int doctorId = 4;

        var result = _service.GetPetsByDoctorOrderedByName(doctorId).ToList();

        Assert.Equal(2, result.Count);
        Assert.Collection(result,
            pet1 => Assert.Equal("Рекс", pet1.Name),
            pet2 => Assert.Equal("Шарик", pet2.Name)
        );
    }

    /// <summary>
    /// Проверка: Количество повторных приемов для породы Лабрадор ретривер - BreedId = 1
    /// Из 4 приемов для лабрадора повторными являются ровно 2 приема - Appointments 2 и 8
    /// </summary>
    [Fact]
    public void GetFollowUpAppointmentsCountByBreed_ReturnsExactCountOfFollowUps()
    {
        const int labradorBreedId = 1;

        var actualCount = _service.GetFollowUpAppointmentsCountByBreed(labradorBreedId);

        Assert.Equal(2, actualCount);
    }

    /// <summary>
    /// Проверка: Вывод владельцев, имеющих более одного питомца, упорядоченных по ФИО
    /// У Иванова (2 питомца) и Петрова (2 питомца)
    /// Ожидаем ровно 2 владельца в алфавитном порядке
    /// </summary>
    [Fact]
    public void GetOwnersWithMultiplePetsOrderedByName_ReturnsFilteredAndAlphabeticallySortedOwners()
    {
        var result = _service.GetOwnersWithMultiplePetsOrderedByName().ToList();

        Assert.Equal(2, result.Count);
        Assert.Collection(result,
            owner1 => Assert.Equal("Иванов Иван Иванович", owner1.FullName),
            owner2 => Assert.Equal("Петров Петр Петрович", owner2.FullName)
        );
    }

    /// <summary>
    /// Проверка: Вывод приемов за сентябрь 2026 года в кабинете 101
    /// В кабинете 101 было 4 приема: 3 в сентябре и 1 в августе
    /// Ожидаем 3 приема, относящихся к сентябрю
    /// </summary>
    [Fact]
    public void GetCurrentMonthAppointmentsByRoom_ReturnsOnlyAppointmentsForSpecifiedMonthAndRoom()
    {
        const int targetRoom = 101;
        var septemberDate = new DateTime(2026, 9, 1);

        var result = _service.GetCurrentMonthAppointmentsByRoom(targetRoom, septemberDate).ToList();

        Assert.Equal(3, result.Count);
        Assert.All(result, a =>
        {
            Assert.Equal(targetRoom, a.RoomNumber);
            Assert.Equal(2026, a.DateTime.Year);
            Assert.Equal(9, a.DateTime.Month);
        });
    }
}
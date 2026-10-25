using Microsoft.AspNetCore.Mvc;
using VetClinic.Application.DTOs;
using VetClinic.Domain.Entities;
using VetClinic.Domain.Interfaces;

namespace VetClinic.WebApi.Controllers;

/// <summary>
/// Управление приемами ветеринарной клиники
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AppointmentsController(
    IRepository<Appointment> repository,
    IRepository<Pet> petRepository,
    IRepository<Veterinarian> vetRepository,
    IVetClinicService clinicService) : ControllerBase
{
    /// <summary>
    /// Получить все приемы
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<AppointmentDto>> GetAll()
    {
        var appointments = repository.GetAll().Select(a => new AppointmentDto
        {
            Id = a.Id,
            DateTime = a.DateTime,
            RoomNumber = a.RoomNumber,
            IsFollowUp = a.IsFollowUp,
            PetId = a.PetId,
            PetName = a.Pet?.Name,
            VeterinarianId = a.VeterinarianId,
            VeterinarianName = a.Veterinarian?.FullName
        });

        return Ok(appointments);
    }

    /// <summary>
    /// Получить количество повторных приемов для выбранной породы
    /// </summary>
    [HttpGet("follow-up-count")]
    public ActionResult<int> GetFollowUpCount([FromQuery] int breedId)
    {
        var count = clinicService.GetFollowUpAppointmentsCountByBreed(breedId);
        return Ok(count);
    }

    /// <summary>
    /// Получить приемы за указанный месяц в выбранном кабинете
    /// </summary>
    [HttpGet("by-room")]
    public ActionResult<IEnumerable<AppointmentDto>> GetByRoomAndMonth(
        [FromQuery] int roomNumber,
        [FromQuery] DateTime date)
    {
        var appointments = clinicService.GetCurrentMonthAppointmentsByRoom(roomNumber, date).Select(a => new AppointmentDto
        {
            Id = a.Id,
            DateTime = a.DateTime,
            RoomNumber = a.RoomNumber,
            IsFollowUp = a.IsFollowUp,
            PetId = a.PetId,
            PetName = a.Pet?.Name,
            VeterinarianId = a.VeterinarianId,
            VeterinarianName = a.Veterinarian?.FullName
        });

        return Ok(appointments);
    }

    /// <summary>
    /// Получить прием по идентификатору
    /// </summary>
    [HttpGet("{id:int}")]
    public ActionResult<AppointmentDto> GetById(int id)
    {
        var a = repository.GetById(id);
        if (a is null)
        {
            return NotFound();
        }

        return Ok(new AppointmentDto
        {
            Id = a.Id,
            DateTime = a.DateTime,
            RoomNumber = a.RoomNumber,
            IsFollowUp = a.IsFollowUp,
            PetId = a.PetId,
            PetName = a.Pet?.Name,
            VeterinarianId = a.VeterinarianId,
            VeterinarianName = a.Veterinarian?.FullName
        });
    }

    /// <summary>
    /// Обновить данные приема
    /// </summary>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] CreateAppointmentDto dto)
    {
        var appointment = repository.GetById(id);
        if (appointment is null)
        {
            return NotFound();
        }

        var pet = petRepository.GetById(dto.PetId);
        var vet = vetRepository.GetById(dto.VeterinarianId);

        if (pet is null || vet is null)
        {
            return BadRequest();
        }

        appointment.DateTime = dto.DateTime;
        appointment.RoomNumber = dto.RoomNumber;
        appointment.IsFollowUp = dto.IsFollowUp;
        appointment.PetId = dto.PetId;
        appointment.Pet = pet;
        appointment.VeterinarianId = dto.VeterinarianId;
        appointment.Veterinarian = vet;

        repository.Update(appointment);

        return NoContent();
    }

    /// <summary>
    /// Записать на прием
    /// </summary>
    [HttpPost]
    public ActionResult<AppointmentDto> Create([FromBody] CreateAppointmentDto dto)
    {
        var pet = petRepository.GetById(dto.PetId);
        var vet = vetRepository.GetById(dto.VeterinarianId);

        if (pet is null || vet is null)
        {
            return BadRequest();
        }

        var appointment = new Appointment
        {
            DateTime = dto.DateTime,
            RoomNumber = dto.RoomNumber,
            IsFollowUp = dto.IsFollowUp,
            PetId = dto.PetId,
            Pet = pet,
            VeterinarianId = dto.VeterinarianId,
            Veterinarian = vet
        };

        repository.Add(appointment);

        return Ok(new AppointmentDto
        {
            Id = appointment.Id,
            DateTime = appointment.DateTime,
            RoomNumber = appointment.RoomNumber,
            IsFollowUp = appointment.IsFollowUp,
            PetId = pet.Id,
            PetName = pet.Name,
            VeterinarianId = vet.Id,
            VeterinarianName = vet.FullName
        });
    }

    /// <summary>
    /// Отменить прием
    /// </summary>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return repository.Delete(id) ? NoContent() : NotFound();
    }
}
using Microsoft.AspNetCore.Mvc;
using VetClinic.Application.DTOs;
using VetClinic.Domain.Entities;
using VetClinic.Domain.Enums;
using VetClinic.Domain.Interfaces;

namespace VetClinic.WebApi.Controllers;

/// <summary>
/// Управление ветеринарнами
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VeterinariansController(
    IRepository<Veterinarian> repository,
    IRepository<Specialization> specializationRepository,
    IVetClinicService clinicService) : ControllerBase
{
    /// <summary>
    /// Получить список всех ветеринаров
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<VeterinarianDto>> GetAll()
    {
        var vets = repository.GetAll().Select(v => new VeterinarianDto
        {
            Id = v.Id,
            PassportNumber = v.PassportNumber,
            FullName = v.FullName,
            BirthYear = v.BirthYear,
            ExperienceYears = v.ExperienceYears,
            SpecializationId = v.SpecializationId,
            SpecializationName = v.Specialization?.Name
        });

        return Ok(vets);
    }

    /// <summary>
    /// Получить ветеринаров с профилем по выбранному виду животных
    /// </summary>
    [HttpGet("by-species")]
    public ActionResult<IEnumerable<VeterinarianDto>> GetBySpecies([FromQuery] AnimalSpecies species)
    {
        var vets = clinicService.GetVeterinariansBySpecies(species).Select(v => new VeterinarianDto
        {
            Id = v.Id,
            PassportNumber = v.PassportNumber,
            FullName = v.FullName,
            BirthYear = v.BirthYear,
            ExperienceYears = v.ExperienceYears,
            SpecializationId = v.SpecializationId,
            SpecializationName = v.Specialization?.Name
        });

        return Ok(vets);
    }

    /// <summary>
    /// Получить врача по идентификатору
    /// </summary>
    [HttpGet("{id:int}")]
    public ActionResult<VeterinarianDto> GetById(int id)
    {
        var vet = repository.GetById(id);
        if (vet is null)
        {
            return NotFound();
        }

        return Ok(new VeterinarianDto
        {
            Id = vet.Id,
            PassportNumber = vet.PassportNumber,
            FullName = vet.FullName,
            BirthYear = vet.BirthYear,
            ExperienceYears = vet.ExperienceYears,
            SpecializationId = vet.SpecializationId,
            SpecializationName = vet.Specialization?.Name
        });
    }

    /// <summary>
    /// Добавить нового врача
    /// </summary>
    [HttpPost]
    public ActionResult<VeterinarianDto> Create([FromBody] CreateVeterinarianDto dto)
    {
        var spec = specializationRepository.GetById(dto.SpecializationId);
        if (spec is null)
        {
            return BadRequest();
        }

        var vet = new Veterinarian
        {
            PassportNumber = dto.PassportNumber,
            FullName = dto.FullName,
            BirthYear = dto.BirthYear,
            ExperienceYears = dto.ExperienceYears,
            SpecializationId = dto.SpecializationId,
            Specialization = spec
        };

        repository.Add(vet);

        return CreatedAtAction(nameof(GetById), new { id = vet.Id }, new VeterinarianDto
        {
            Id = vet.Id,
            PassportNumber = vet.PassportNumber,
            FullName = vet.FullName,
            BirthYear = vet.BirthYear,
            ExperienceYears = vet.ExperienceYears,
            SpecializationId = spec.Id,
            SpecializationName = spec.Name
        });
    }

    /// <summary>
    /// Обновить данные врача
    /// </summary>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] CreateVeterinarianDto dto)
    {
        var vet = repository.GetById(id);
        if (vet is null)
        {
            return NotFound();
        }

        var spec = specializationRepository.GetById(dto.SpecializationId);
        if (spec is null)
        {
            return BadRequest();
        }

        vet.PassportNumber = dto.PassportNumber;
        vet.FullName = dto.FullName;
        vet.BirthYear = dto.BirthYear;
        vet.ExperienceYears = dto.ExperienceYears;
        vet.SpecializationId = dto.SpecializationId;
        vet.Specialization = spec;

        repository.Update(vet);

        return NoContent();
    }

    /// <summary>
    /// Удалить врача
    /// </summary>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return repository.Delete(id) ? NoContent() : NotFound();
    }
}
using Microsoft.AspNetCore.Mvc;
using VetClinic.Application.DTOs;
using VetClinic.Domain.Entities;
using VetClinic.Domain.Interfaces;

namespace VetClinic.WebApi.Controllers;

/// <summary>
/// Управление справочником специализаций врачей
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SpecializationsController(IRepository<Specialization> repository) : ControllerBase
{
    /// <summary>
    /// Получить список всех специализаций
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<SpecializationDto>> GetAll()
    {
        var specializations = repository.GetAll().Select(s => new SpecializationDto
        {
            Id = s.Id,
            Name = s.Name,
            TargetSpecies = s.TargetSpecies
        });

        return Ok(specializations);
    }

    /// <summary>
    /// Получить специализацию по идентификатору
    /// </summary>
    [HttpGet("{id:int}")]
    public ActionResult<SpecializationDto> GetById(int id)
    {
        var specialization = repository.GetById(id);
        if (specialization is null)
        {
            return NotFound();
        }

        return Ok(new SpecializationDto
        {
            Id = specialization.Id,
            Name = specialization.Name,
            TargetSpecies = specialization.TargetSpecies
        });
    }

    /// <summary>
    /// Добавить новую специализацию
    /// </summary>
    [HttpPost]
    public ActionResult<SpecializationDto> Create([FromBody] CreateSpecializationDto dto)
    {
        var specialization = new Specialization
        {
            Name = dto.Name,
            TargetSpecies = dto.TargetSpecies
        };

        repository.Add(specialization);

        return CreatedAtAction(nameof(GetById), new { id = specialization.Id }, new SpecializationDto
        {
            Id = specialization.Id,
            Name = specialization.Name,
            TargetSpecies = specialization.TargetSpecies
        });
    }

    /// <summary>
    /// Обновить специализацию
    /// </summary>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] CreateSpecializationDto dto)
    {
        var specialization = repository.GetById(id);
        if (specialization is null)
        {
            return NotFound();
        }

        specialization.Name = dto.Name;
        specialization.TargetSpecies = dto.TargetSpecies;
        repository.Update(specialization);

        return NoContent();
    }

    /// <summary>
    /// Удалить специализацию
    /// </summary>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return repository.Delete(id) ? NoContent() : NotFound();
    }
}
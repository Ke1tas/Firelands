using Microsoft.AspNetCore.Mvc;
using VetClinic.Application.DTOs;
using VetClinic.Domain.Entities;
using VetClinic.Domain.Interfaces;

namespace VetClinic.WebApi.Controllers;

/// <summary>
/// Управление справочником пород
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BreedsController(IRepository<Breed> repository) : ControllerBase
{
    /// <summary>
    /// Получить все породы
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<BreedDto>> GetAll()
    {
        var breeds = repository.GetAll().Select(b => new BreedDto
        {
            Id = b.Id,
            Name = b.Name,
            Species = b.Species
        });

        return Ok(breeds);
    }

    /// <summary>
    /// Получить породу по идентификатору
    /// </summary>
    [HttpGet("{id:int}")]
    public ActionResult<BreedDto> GetById(int id)
    {
        var breed = repository.GetById(id);
        if (breed is null)
        {
            return NotFound();
        }

        return Ok(new BreedDto
        {
            Id = breed.Id,
            Name = breed.Name,
            Species = breed.Species
        });
    }

    /// <summary>
    /// Добавить новую породу
    /// </summary>
    [HttpPost]
    public ActionResult<BreedDto> Create([FromBody] CreateBreedDto dto)
    {
        var breed = new Breed
        {
            Name = dto.Name,
            Species = dto.Species
        };

        repository.Add(breed);

        return CreatedAtAction(nameof(GetById), new { id = breed.Id }, new BreedDto
        {
            Id = breed.Id,
            Name = breed.Name,
            Species = breed.Species
        });
    }

    /// <summary>
    /// Обновить существующую породу
    /// </summary>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] CreateBreedDto dto)
    {
        var breed = repository.GetById(id);
        if (breed is null)
        {
            return NotFound();
        }

        breed.Name = dto.Name;
        breed.Species = dto.Species;
        repository.Update(breed);

        return NoContent();
    }

    /// <summary>
    /// Удалить породу
    /// </summary>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return repository.Delete(id) ? NoContent() : NotFound();
    }
}
using Microsoft.AspNetCore.Mvc;
using VetClinic.Application.DTOs;
using VetClinic.Domain.Entities;
using VetClinic.Domain.Interfaces;

namespace VetClinic.WebApi.Controllers;

/// <summary>
/// Управление владельцами животных
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OwnersController(
    IRepository<Owner> repository,
    IVetClinicService clinicService) : ControllerBase
{
    /// <summary>
    /// Получить список всех владельцев
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<OwnerDto>> GetAll()
    {
        var owners = repository.GetAll().Select(o => new OwnerDto
        {
            Id = o.Id,
            FullName = o.FullName,
            Phone = o.Phone,
            Address = o.Address,
            PetsCount = o.Pets.Count
        });

        return Ok(owners);
    }

    /// <summary>
    /// Получить владельцев с более чем одним питомцем
    /// </summary>
    [HttpGet("multiple-pets")]
    public ActionResult<IEnumerable<OwnerDto>> GetOwnersWithMultiplePets()
    {
        var owners = clinicService.GetOwnersWithMultiplePetsOrderedByName().Select(o => new OwnerDto
        {
            Id = o.Id,
            FullName = o.FullName,
            Phone = o.Phone,
            Address = o.Address,
            PetsCount = o.Pets.Count
        });

        return Ok(owners);
    }

    /// <summary>
    /// Получить владельца по идентификатору
    /// </summary>
    [HttpGet("{id:int}")]
    public ActionResult<OwnerDto> GetById(int id)
    {
        var owner = repository.GetById(id);
        if (owner is null)
        {
            return NotFound();
        }

        return Ok(new OwnerDto
        {
            Id = owner.Id,
            FullName = owner.FullName,
            Phone = owner.Phone,
            Address = owner.Address,
            PetsCount = owner.Pets.Count
        });
    }

    /// <summary>
    /// Добавить владельца
    /// </summary>
    [HttpPost]
    public ActionResult<OwnerDto> Create([FromBody] CreateOwnerDto dto)
    {
        var owner = new Owner
        {
            FullName = dto.FullName,
            Phone = dto.Phone,
            Address = dto.Address
        };

        repository.Add(owner);

        return CreatedAtAction(nameof(GetById), new { id = owner.Id }, new OwnerDto
        {
            Id = owner.Id,
            FullName = owner.FullName,
            Phone = owner.Phone,
            Address = owner.Address,
            PetsCount = 0
        });
    }

    /// <summary>
    /// Обновить данные владельца
    /// </summary>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] CreateOwnerDto dto)
    {
        var owner = repository.GetById(id);
        if (owner is null)
        {
            return NotFound();
        }

        owner.FullName = dto.FullName;
        owner.Phone = dto.Phone;
        owner.Address = dto.Address;

        repository.Update(owner);

        return NoContent();
    }

    /// <summary>
    /// Удалить владельца
    /// </summary>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return repository.Delete(id) ? NoContent() : NotFound();
    }
}
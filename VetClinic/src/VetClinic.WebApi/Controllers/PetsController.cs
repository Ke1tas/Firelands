using Microsoft.AspNetCore.Mvc;
using VetClinic.Application.DTOs;
using VetClinic.Domain.Entities;
using VetClinic.Domain.Interfaces;

namespace VetClinic.WebApi.Controllers;

/// <summary>
/// Управление питомцами
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PetsController(
    IRepository<Pet> repository,
    IRepository<Breed> breedRepository,
    IRepository<Owner> ownerRepository,
    IVetClinicService clinicService) : ControllerBase
{
    /// <summary>
    /// Получить список всех питомцев
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<PetDto>> GetAll()
    {
        var pets = repository.GetAll().Select(p => new PetDto
        {
            Id = p.Id,
            Name = p.Name,
            Species = p.Species,
            BreedId = p.BreedId,
            BreedName = p.Breed?.Name,
            BirthDate = p.BirthDate,
            WeightKg = p.WeightKg,
            OwnerId = p.OwnerId,
            OwnerName = p.Owner?.FullName
        });

        return Ok(pets);
    }

    /// <summary>
    /// Получить питомцев записанных к выбранному врачу
    /// </summary>
    [HttpGet("by-doctor/{doctorId:int}")]
    public ActionResult<IEnumerable<PetDto>> GetPetsByDoctor(int doctorId)
    {
        var pets = clinicService.GetPetsByDoctorOrderedByName(doctorId).Select(p => new PetDto
        {
            Id = p.Id,
            Name = p.Name,
            Species = p.Species,
            BreedId = p.BreedId,
            BreedName = p.Breed?.Name,
            BirthDate = p.BirthDate,
            WeightKg = p.WeightKg,
            OwnerId = p.OwnerId,
            OwnerName = p.Owner?.FullName
        });

        return Ok(pets);
    }

    /// <summary>
    /// Получить питомца по идентификатору
    /// </summary>
    [HttpGet("{id:int}")]
    public ActionResult<PetDto> GetById(int id)
    {
        var p = repository.GetById(id);
        if (p is null)
        {
            return NotFound();
        }

        return Ok(new PetDto
        {
            Id = p.Id,
            Name = p.Name,
            Species = p.Species,
            BreedId = p.BreedId,
            BreedName = p.Breed?.Name,
            BirthDate = p.BirthDate,
            WeightKg = p.WeightKg,
            OwnerId = p.OwnerId,
            OwnerName = p.Owner?.FullName
        });
    }

    /// <summary>
    /// Обновить данные питомца
    /// </summary>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] CreatePetDto dto)
    {
        var pet = repository.GetById(id);
        if (pet is null)
        {
            return NotFound();
        }

        var breed = breedRepository.GetById(dto.BreedId);
        var owner = ownerRepository.GetById(dto.OwnerId);

        if (breed is null || owner is null)
        {
            return BadRequest();
        }

        pet.Name = dto.Name;
        pet.Species = dto.Species;
        pet.BreedId = dto.BreedId;
        pet.Breed = breed;
        pet.BirthDate = dto.BirthDate;
        pet.WeightKg = dto.WeightKg;
        pet.OwnerId = dto.OwnerId;
        pet.Owner = owner;

        repository.Update(pet);

        return NoContent();
    }

    /// <summary>
    /// Зарегистрировать нового питомца
    /// </summary>
    [HttpPost]
    public ActionResult<PetDto> Create([FromBody] CreatePetDto dto)
    {
        var breed = breedRepository.GetById(dto.BreedId);
        var owner = ownerRepository.GetById(dto.OwnerId);

        if (breed is null || owner is null)
        {
            return BadRequest();
        }

        var pet = new Pet
        {
            Name = dto.Name,
            Species = dto.Species,
            BreedId = dto.BreedId,
            Breed = breed,
            BirthDate = dto.BirthDate,
            WeightKg = dto.WeightKg,
            OwnerId = dto.OwnerId,
            Owner = owner
        };

        repository.Add(pet);
        owner.Pets.Add(pet);

        return Ok(new PetDto
        {
            Id = pet.Id,
            Name = pet.Name,
            Species = pet.Species,
            BreedId = pet.BreedId,
            BreedName = breed.Name,
            BirthDate = pet.BirthDate,
            WeightKg = pet.WeightKg,
            OwnerId = pet.OwnerId,
            OwnerName = owner.FullName
        });
    }

    /// <summary>
    /// Удалить питомца
    /// </summary>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return repository.Delete(id) ? NoContent() : NotFound();
    }
}
using VetClinic.Domain.Enums;

namespace VetClinic.Domain.Entities;

/// <summary>
/// Специализация ветеринарного врача
/// </summary>
public class Specialization
{
    /// <summary>
    /// Уникальный идентификатор специализации
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название специализации
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Профильный вид животных для данной специализации
    /// </summary>
    public AnimalSpecies TargetSpecies { get; set; }
}
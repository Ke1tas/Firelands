using VetClinic.Domain.Enums;

namespace VetClinic.Infrastructure.Seeding;

/// <summary>
/// Справочник данных для генерации
/// </summary>
public static class InitialDataCatalog
{
    public static readonly IReadOnlyDictionary<AnimalSpecies, string[]> BreedsBySpecies = 
        new Dictionary<AnimalSpecies, string[]>
        {
            [AnimalSpecies.Dog] = 
            [
                "Лабрадор ретривер", 
                "Немецкая овчарка", 
                "Французский бульдог", 
                "Сибирский хаски", 
                "Вельш-корги пемброк"
            ],
            [AnimalSpecies.Cat] = 
            [
                "Мейн-кун", 
                "Британская короткошерстная", 
                "Сфинкс", 
                "Сиамская", 
                "Шотландская вислоухая"
            ],
            [AnimalSpecies.Bird] = 
            [
                "Волнистый попугай", 
                "Корелла", 
                "Канарейка", 
                "Неразлучник"
            ],
            [AnimalSpecies.Rodent] = 
            [
                "Джунгарский хомяк", 
                "Сирийский хомяк", 
                "Морская свинка", 
                "Декоративная крыса", 
                "Шиншилла"
            ],
            [AnimalSpecies.Reptile] = 
            [
                "Бородатая агама", 
                "Пятнистый эублефар", 
                "Йеменский хамелеон", 
                "Красноухая черепаха"
            ]
        };

    public static readonly string[] SpecializationTitles =
    [
        "Общая терапия",
        "Хирургия и травматология",
        "Дерматология",
        "Офтальмология",
        "Кардиология",
        "Стоматология",
        "Анестезиология и реанимация",
        "Орнитология"
    ];
}
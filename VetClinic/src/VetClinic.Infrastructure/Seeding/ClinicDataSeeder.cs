using Bogus;
using VetClinic.Domain.Entities;
using VetClinic.Domain.Enums;

namespace VetClinic.Infrastructure.Seeding;

public class ClinicDataSeeder
{
    public List<Breed> Breeds { get; private set; } = [];
    public List<Specialization> Specializations { get; private set; } = [];
    public List<Owner> Owners { get; private set; } = [];
    public List<Pet> Pets { get; private set; } = [];
    public List<Veterinarian> Veterinarians { get; private set; } = [];
    public List<Appointment> Appointments { get; private set; } = [];

    public ClinicDataSeeder(int seed = 67676767){
        Randomizer.Seed = new Random(seed);
        Generate();
    }
    private void Generate(){
        var breedIdCounter = 1;
        var specIdCounter = 1;
        var ownerIdCounter = 1;
        var petIdCounter = 1;
        var vetIdCounter = 1;
        var appointmentIdCounter = 1;

        Breeds = InitialDataCatalog.BreedsBySpecies
        .SelectMany(pair => pair.Value.Select(breedName => new Breed
        {
            Id = breedIdCounter++,
            Species = pair.Key,
            Name = breedName
        }))
        .ToList();

        var specFaker = new Faker<Specialization>("ru")
            .RuleFor(s => s.Id, _ => specIdCounter++)
            .RuleFor(s => s.TargetSpecies, f => f.PickRandom<AnimalSpecies>())
            .RuleFor(s => s.Name, f => f.PickRandom(InitialDataCatalog.SpecializationTitles));
        Specializations = specFaker.Generate(10);

        var ownerFaker = new Faker<Owner>("ru")
            .RuleFor(o => o.Id, _ => ownerIdCounter++)
            .RuleFor(o => o.FullName, f => f.Name.FullName())
            .RuleFor(o => o.Address, f => f.Address.FullAddress())
            .RuleFor(o => o.Phone, f => f.Phone.PhoneNumber("+7-9##-###-##-##"));
        Owners = ownerFaker.Generate(12);

        var petFaker = new Faker<Pet>("ru")
            .RuleFor(p => p.Id, _ => petIdCounter++)
            .RuleFor(p => p.Name, f => f.Name.FirstName())
            .RuleFor(p => p.BirthDate, f => DateOnly.FromDateTime(f.Date.Past(8)))
            .RuleFor(p => p.WeightKg, f => Math.Round(f.Random.Decimal(0.5m, 40m), 2))
            .CustomInstantiator(f => {
                var breed = f.PickRandom(Breeds);
                var owner = f.PickRandom(Owners);
                var pet = new Pet
                {
                    BreedId = breed.Id,
                    Breed = breed,
                    Species = breed.Species,
                    OwnerId = owner.Id,
                    Owner = owner
                };
                owner.Pets.Add(pet);
                return pet;
            });
        Pets = petFaker.Generate(15);

        var vetFaker = new Faker<Veterinarian>("ru")
            .RuleFor(v => v.Id, _ => vetIdCounter++)
            .RuleFor(v => v.PassportNumber, f => f.Random.Replace("## ## ######"))
            .RuleFor(v => v.FullName, f => f.Name.FullName())
            .RuleFor(v => v.BirthYear, f => f.Date.Past(35, DateTime.Now.AddYears(-24)).Year)
            .RuleFor(v => v.ExperienceYears, f => f.Random.Int(1, 30))
            .CustomInstantiator(f =>
            {
                var spec = f.PickRandom(Specializations);
                return new Veterinarian
                {
                    SpecializationId = spec.Id,
                    Specialization = spec
                };
            });
        Veterinarians = vetFaker.Generate(12);

        var appointmentFaker = new Faker<Appointment>("ru")
            .RuleFor(a => a.Id, _ => appointmentIdCounter++)
            .RuleFor(a => a.DateTime, f => f.Date.Between(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1)))
            .RuleFor(a => a.RoomNumber, f => f.Random.Int(101, 105))
            .RuleFor(a => a.IsFollowUp, f => f.Random.Bool(0.35f))
            .CustomInstantiator(f =>
            {
                var pet = f.PickRandom(Pets);
                var vet = f.PickRandom(Veterinarians);
                var appointment = new Appointment
                {
                    PetId = pet.Id,
                    Pet = pet,
                    VeterinarianId = vet.Id,
                    Veterinarian = vet
                };
                pet.Appointments.Add(appointment);
                vet.Appointments.Add(appointment);
                return appointment;
            });
        Appointments = appointmentFaker.Generate(40);
    }
}
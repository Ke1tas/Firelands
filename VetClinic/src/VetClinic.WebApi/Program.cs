using VetClinic.Application.Services;
using VetClinic.Domain.Entities;
using VetClinic.Domain.Interfaces;
using VetClinic.Infrastructure.Repositories;
using VetClinic.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

// Инициализация начальных данных через сидер
var seeder = new ClinicDataSeeder();

// Регистрация репозиториев в памяти как Singleton
builder.Services.AddSingleton<IRepository<Breed>>(_ => new InMemoryRepository<Breed>(seeder.Breeds));
builder.Services.AddSingleton<IRepository<Specialization>>(_ => new InMemoryRepository<Specialization>(seeder.Specializations));
builder.Services.AddSingleton<IRepository<Owner>>(_ => new InMemoryRepository<Owner>(seeder.Owners));
builder.Services.AddSingleton<IRepository<Pet>>(_ => new InMemoryRepository<Pet>(seeder.Pets));
builder.Services.AddSingleton<IRepository<Veterinarian>>(_ => new InMemoryRepository<Veterinarian>(seeder.Veterinarians));
builder.Services.AddSingleton<IRepository<Appointment>>(_ => new InMemoryRepository<Appointment>(seeder.Appointments));

// Регистрация аналитического сервиса
builder.Services.AddScoped<IVetClinicService>(sp =>
{
    var vets = sp.GetRequiredService<IRepository<Veterinarian>>().GetAll();
    var apps = sp.GetRequiredService<IRepository<Appointment>>().GetAll();
    var owners = sp.GetRequiredService<IRepository<Owner>>().GetAll();
    return new VetClinicService(vets, apps, owners);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
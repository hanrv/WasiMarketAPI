using Microsoft.EntityFrameworkCore;
using WasiMarketAPI.Data;
using WasiMarketAPI.Repositories;
using WasiMarketAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection string not found.");

// Registro de ORM: Entity Framework Core
builder.Services.AddDbContext<WasiMarketContext>(options =>
    options.UseSqlServer(connectionString));

// Registro de MicroORM: Dapper Repository
builder.Services.AddSingleton<IProductoDapperRepository, ProductoDapperRepository>();

// Registro de Servicio de Carrito en Memoria (Singleton para conservar estado de sesión local)
builder.Services.AddSingleton<ICarritoService, CarritoService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
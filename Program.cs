using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WasiMarketAPI.Data;
using WasiMarketAPI.Repositories;
using WasiMarketAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Cadena de conexión y DbContext
builder.Services.AddDbContext<WasiMarketContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Registro de Repositorios y Servicios existentes
// 2. Registro de Repositorios y Servicios existentes
builder.Services.AddScoped<IProductoDapperRepository, ProductoDapperRepository>();
builder.Services.AddScoped<ICarritoService, CarritoService>(); 

// 3. Configuración del Middleware de Autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)
            ),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();
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

// El orden de ejecución es estricto
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WasiMarketAPI.Data;
using WasiMarketAPI.DTOs;
using WasiMarketAPI.Models;

namespace WasiMarketAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly WasiMarketContext _context;
    private readonly IConfiguration _config;

    public AuthController(WasiMarketContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // 1. REGISTRO: Encripta la contraseña usando BCrypt antes de guardar en la BD
    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] RegisterDTO peticion)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Email == peticion.Email))
            return BadRequest("El correo ya se encuentra registrado.");

        // Hasheo seguro unidireccional con Salt generado automáticamente
        string passwordHashBD = BCrypt.Net.BCrypt.HashPassword(peticion.Password);

        var nuevoUsuario = new Usuario
        {
            Nombre = peticion.Nombre,
            Email = peticion.Email,
            PasswordHash = passwordHashBD,
            Rol = string.IsNullOrWhiteSpace(peticion.Rol) ? "Cliente" : peticion.Rol
        };

        _context.Usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();

        return Ok(new { Mensaje = "Usuario registrado exitosamente" });
    }

    // 2 y 3. LOGIN: Idéntico al diseño demo, validando credenciales contra la BD
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO peticion)
    {
        // 1. Buscamos usuario y verificamos con BCrypt.Verify()
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == peticion.Email);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(peticion.Password, usuario.PasswordHash))
            return Unauthorized("Credenciales inválidas");

        // 2. Preparamos los datos del Token (Claims)
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()), // ID del usuario
            new Claim(ClaimTypes.Email, peticion.Email),
            new Claim(ClaimTypes.Role, usuario.Rol) // Para Autorización futura
        };

        // 3. Firmamos con la llave secreta del appsettings
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 4. Creamos el Token
        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(double.Parse(_config["JwtSettings:MinutosExpiracion"]!)),
            signingCredentials: creds
        );

        return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}
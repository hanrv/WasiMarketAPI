using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
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

    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] RegisterDTO peticion)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Email == peticion.Email))
            return BadRequest("El correo ya se encuentra registrado.");

        string rolAsignado = string.IsNullOrWhiteSpace(peticion.Rol)
            ? RolesUsuario.Cliente
            : peticion.Rol.Trim();

        if (!RolesUsuario.Todos.Contains(rolAsignado))
        {
            return BadRequest($"Rol inválido. Los roles permitidos son: {string.Join(", ", RolesUsuario.Todos)}");
        }

        string passwordHashBD = BCrypt.Net.BCrypt.HashPassword(peticion.Password); //[cite: 4]

        var nuevoUsuario = new Usuario
        {
            Nombre = peticion.Nombre,
            Email = peticion.Email,
            PasswordHash = passwordHashBD,
            Rol = rolAsignado
        };

        _context.Usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Mensaje = "Usuario registrado exitosamente",
            Rol = rolAsignado
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO peticion)
    {
        // 1. Buscamos usuario y verificamos con BCrypt.Verify()[cite: 8, 12]
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == peticion.Email);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(peticion.Password, usuario.PasswordHash)) //[cite: 4, 8]
            return Unauthorized("Credenciales inválidas"); //[cite: 8, 12]

        // 2. Preparamos los datos del Token (Claims)[cite: 8, 12]
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()), // ID del usuario[cite: 8, 12]
            new Claim(ClaimTypes.Email, peticion.Email), //[cite: 8, 12]
            new Claim(ClaimTypes.Role, usuario.Rol) // Para Autorización futura[cite: 8, 12]
        };

        // 3. Firmamos con la llave secreta del appsettings[cite: 8, 12]
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!)); //[cite: 8, 12]
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //[cite: 8, 12]

        // 4. Creamos el Token[cite: 8, 12]
        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"], //[cite: 8, 12]
            audience: _config["JwtSettings:Audience"], //[cite: 8, 12]
            claims: claims, //[cite: 8, 12]
            expires: DateTime.Now.AddMinutes(double.Parse(_config["JwtSettings:MinutosExpiracion"]!)), //[cite: 8, 12]
            signingCredentials: creds //[cite: 8, 12]
        );

        return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) }); //[cite: 8, 12]
    }

    // Endpoint accesible por cualquier usuario autenticado[cite: 9]
    [Authorize] //[cite: 9]
    [HttpGet("perfil")]
    public IActionResult GetPerfil()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value; //[cite: 9]
        var rol = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new
        {
            Mensaje = "Perfil autenticado",
            Id = id,
            Email = email,
            Rol = rol
        });
    }

    // Endpoint restringido exclusivamente a Administradores[cite: 2, 9]
    [Authorize(Roles = RolesUsuario.Administrador)] //[cite: 9]
    [HttpGet("solo-admin")]
    public IActionResult SoloAdmin()
    {
        return Ok("Acceso exclusivo concedido al Administrador.");
    }

    // Endpoint restringido a Clientes[cite: 2, 9]
    [Authorize(Roles = RolesUsuario.Cliente)] //[cite: 9]
    [HttpGet("solo-cliente")]
    public IActionResult SoloCliente()
    {
        return Ok("Acceso concedido al portal de Cliente.");
    }
}
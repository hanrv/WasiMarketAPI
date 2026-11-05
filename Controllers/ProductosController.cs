using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WasiMarketAPI.Data;
using WasiMarketAPI.DTOs;
using WasiMarketAPI.Models;
using WasiMarketAPI.Repositories;

namespace WasiMarketAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly WasiMarketContext _context;
    private readonly IProductoDapperRepository _dapperRepo;

    public ProductosController(WasiMarketContext context, IProductoDapperRepository dapperRepo)
    {
        _context = context;
        _dapperRepo = dapperRepo;
    }

    // MicroORM (Dapper): Consulta masiva de catálogo activo
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductoLecturaDTO>>> GetCatalogo()
    {
        var productos = await _dapperRepo.ObtenerCatalogoActivoAsync();
        return Ok(productos);
    }

    // MicroORM (Dapper): Consulta puntual por ID
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductoLecturaDTO>> GetPorId(int id)
    {
        var producto = await _dapperRepo.ObtenerPorIdAsync(id);
        if (producto is null)
            return NotFound(new { Mensaje = $"Producto con ID {id} no encontrado." });

        return Ok(producto);
    }

    // ORM (EF Core): Inserción con validación de claves foráneas
    [HttpPost]
    public async Task<ActionResult<ProductoLecturaDTO>> CrearProducto([FromBody] ProductoCreateDTO dto)
    {
        try
        {
            var nuevo = new Producto
            {
                IdTienda = dto.IdTienda,
                IdCategoria = dto.IdCategoria,
                NombreProducto = dto.NombreProducto,
                Descripcion = dto.Descripcion,
                PrecioUnitario = dto.PrecioUnitario,
                StockDisponible = dto.StockDisponible,
                StockMinimo = dto.StockMinimo,
                UnidadMedida = dto.UnidadMedida,
                ImagenUrl = dto.ImagenUrl,
                EstadoActivo = true
            };

            _context.Productos.Add(nuevo);
            await _context.SaveChangesAsync();

            var lecturaDto = new ProductoLecturaDTO(
                nuevo.IdProducto,
                nuevo.IdTienda,
                nuevo.IdCategoria,
                nuevo.NombreProducto,
                nuevo.Descripcion,
                nuevo.PrecioUnitario,
                nuevo.StockDisponible,
                nuevo.StockMinimo,
                nuevo.UnidadMedida,
                nuevo.ImagenUrl,
                nuevo.EstadoActivo
            );

            return CreatedAtAction(nameof(GetPorId), new { id = nuevo.IdProducto }, lecturaDto);
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(new
            {
                Mensaje = "No se pudo registrar el producto en la base de datos.",
                Detalle = "Verifica que 'idTienda' e 'idCategoria' existan previamente en WasiMarketDB.",
                ErrorSql = ex.InnerException?.Message ?? ex.Message
            });
        }
    }

    // ORM (EF Core): Modificación con captura de restricciones
    [HttpPut("{id:int}")]
    public async Task<IActionResult> ActualizarProducto(int id, [FromBody] ProductoCreateDTO dto)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto is null)
            return NotFound(new { Mensaje = $"Producto con ID {id} no encontrado." });

        try
        {
            producto.IdTienda = dto.IdTienda;
            producto.IdCategoria = dto.IdCategoria;
            producto.NombreProducto = dto.NombreProducto;
            producto.Descripcion = dto.Descripcion;
            producto.PrecioUnitario = dto.PrecioUnitario;
            producto.StockDisponible = dto.StockDisponible;
            producto.StockMinimo = dto.StockMinimo;
            producto.UnidadMedida = dto.UnidadMedida;
            producto.ImagenUrl = dto.ImagenUrl;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(new
            {
                Mensaje = "No se pudo actualizar el producto.",
                Detalle = "Verifica que los IDs de tienda/categoría existan y cumplan las restricciones de tabla.",
                ErrorSql = ex.InnerException?.Message ?? ex.Message
            });
        }
    }

    // ORM (EF Core): Baja lógica de producto
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DesactivarProducto(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto is null)
            return NotFound(new { Mensaje = $"Producto con ID {id} no encontrado." });

        producto.EstadoActivo = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
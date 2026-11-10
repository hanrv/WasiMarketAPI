using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WasiMarketAPI.Data;
using WasiMarketAPI.DTOs;
using WasiMarketAPI.Models;
using WasiMarketAPI.Repositories;

namespace WasiMarketAPI.Controllers
{
    [Authorize]
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

        // 1. GET: Consulta privada del catálogo activo (vía Dapper)
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var productos = await _dapperRepo.ObtenerCatalogoActivoAsync();
            return Ok(productos);
        }

        // 2. GET: Consulta privada por ID (vía Dapper)
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var producto = await _dapperRepo.ObtenerPorIdAsync(id);
            if (producto == null)
            {
                return NotFound(new { mensaje = $"Producto con ID {id} no encontrado." });
            }

            return Ok(producto);
        }

        // 3. POST: Registro de nuevo producto (vía EF Core)
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ProductoCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nuevoProducto = new Producto
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

            _context.Productos.Add(nuevoProducto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoProducto.IdProducto }, nuevoProducto);
        }

        // 4. PUT: Actualización completa del producto (vía EF Core)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ProductoCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productoExistente = await _context.Productos.FindAsync(id);
            if (productoExistente == null)
            {
                return NotFound(new { mensaje = $"Producto con ID {id} no existe." });
            }

            productoExistente.IdTienda = dto.IdTienda;
            productoExistente.IdCategoria = dto.IdCategoria;
            productoExistente.NombreProducto = dto.NombreProducto;
            productoExistente.Descripcion = dto.Descripcion;
            productoExistente.PrecioUnitario = dto.PrecioUnitario;
            productoExistente.StockDisponible = dto.StockDisponible;
            productoExistente.StockMinimo = dto.StockMinimo;
            productoExistente.UnidadMedida = dto.UnidadMedida;
            productoExistente.ImagenUrl = dto.ImagenUrl;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Producto actualizado correctamente.", producto = productoExistente });
        }

        // 5. PATCH: Actualización parcial (precio unitario)
        [HttpPatch("{id:int}/precio")]
        public async Task<IActionResult> ActualizarPrecio(int id, [FromBody] ActualizarPrecioDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound(new { mensaje = $"Producto con ID {id} no encontrado." });
            }

            producto.PrecioUnitario = dto.NuevoPrecio;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Precio actualizado con éxito.",
                idProducto = producto.IdProducto,
                nuevoPrecio = producto.PrecioUnitario
            });
        }

        // 6. DELETE: Baja lógica del producto
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound(new { mensaje = $"Producto con ID {id} no encontrado." });
            }

            producto.EstadoActivo = false;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Producto con ID {id} dado de baja exitosamente." });
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using WasiMarketAPI.DTOs;
using WasiMarketAPI.Services;

namespace WasiMarketAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarritoController : ControllerBase
{
    private readonly ICarritoService _carritoService;

    public CarritoController(ICarritoService carritoService)
    {
        _carritoService = carritoService;
    }

    [HttpGet]
    public ActionResult<ResumenCompraDTO> ObtenerCarrito()
    {
        return Ok(_carritoService.GenerarResumen());
    }

    [HttpPost("items")]
    public async Task<IActionResult> AgregarItem([FromBody] AgregarItemCarritoDTO request)
    {
        var (exito, mensaje) = await _carritoService.AgregarOModificarItemAsync(request);
        if (!exito) return BadRequest(new { Mensaje = mensaje });

        return Ok(new { Mensaje = mensaje, Carrito = _carritoService.GenerarResumen() });
    }

    [HttpDelete("items/{idProducto:int}")]
    public IActionResult EliminarItem(int idProducto)
    {
        bool eliminado = _carritoService.EliminarItem(idProducto);
        if (!eliminado)
            return NotFound(new { Mensaje = "El producto no existe dentro del carrito actual." });

        return Ok(new { Mensaje = "Producto eliminado.", Carrito = _carritoService.GenerarResumen() });
    }

    [HttpDelete]
    public IActionResult VaciarCarrito()
    {
        _carritoService.VaciarCarrito();
        return NoContent();
    }
}
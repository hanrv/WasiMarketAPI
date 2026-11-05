using WasiMarketAPI.DTOs;

namespace WasiMarketAPI.Services;

public interface ICarritoService
{
    Task<(bool Exito, string Mensaje)> AgregarOModificarItemAsync(AgregarItemCarritoDTO itemDto);
    bool EliminarItem(int idProducto);
    void VaciarCarrito();
    ResumenCompraDTO GenerarResumen();
}
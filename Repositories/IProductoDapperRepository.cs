using WasiMarketAPI.DTOs;

namespace WasiMarketAPI.Repositories;

public interface IProductoDapperRepository
{
    Task<IEnumerable<ProductoLecturaDTO>> ObtenerCatalogoActivoAsync();
    Task<ProductoLecturaDTO?> ObtenerPorIdAsync(int idProducto);
    Task<IEnumerable<ProductoLecturaDTO>> ObtenerPorIdsBatchAsync(IEnumerable<int> ids);
}
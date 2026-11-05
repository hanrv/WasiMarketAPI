using WasiMarketAPI.DTOs;
using WasiMarketAPI.Repositories;

namespace WasiMarketAPI.Services;

public class CarritoService : ICarritoService
{
    // Diccionario para acceso indexado directo O(1) por IdProducto
    private readonly Dictionary<int, CarritoItemDTO> _itemsPorProducto = new();

    // HashSet para garantizar conjunto de productos únicos sin repetición
    private readonly HashSet<int> _productosSeleccionadosIds = new();

    private readonly IProductoDapperRepository _dapperRepo;

    public CarritoService(IProductoDapperRepository dapperRepo)
    {
        _dapperRepo = dapperRepo;
    }

    public async Task<(bool Exito, string Mensaje)> AgregarOModificarItemAsync(AgregarItemCarritoDTO itemDto)
    {
        if (itemDto.Cantidad <= 0)
            return (false, "La cantidad debe ser mayor a 0.");

        var producto = await _dapperRepo.ObtenerPorIdAsync(itemDto.IdProducto);
        if (producto is null || !producto.EstadoActivo)
            return (false, "El producto no existe o está inactivo.");

        int cantidadFinal = itemDto.Cantidad;
        if (_itemsPorProducto.TryGetValue(itemDto.IdProducto, out var itemExistente))
        {
            cantidadFinal += itemExistente.Cantidad;
        }

        if (cantidadFinal > producto.StockDisponible)
            return (false, $"Stock insuficiente. Disponible actual: {producto.StockDisponible}.");

        if (_itemsPorProducto.ContainsKey(itemDto.IdProducto))
        {
            _itemsPorProducto[itemDto.IdProducto].Cantidad = cantidadFinal;
        }
        else
        {
            _itemsPorProducto[itemDto.IdProducto] = new CarritoItemDTO
            {
                IdProducto = producto.IdProducto,
                NombreProducto = producto.NombreProducto,
                IdCategoria = producto.IdCategoria,
                PrecioUnitario = producto.PrecioUnitario,
                Cantidad = itemDto.Cantidad
            };
            _productosSeleccionadosIds.Add(producto.IdProducto);
        }

        return (true, "Producto registrado correctamente en el carrito.");
    }

    public bool EliminarItem(int idProducto)
    {
        if (_itemsPorProducto.Remove(idProducto))
        {
            _productosSeleccionadosIds.Remove(idProducto);
            return true;
        }
        return false;
    }

    public void VaciarCarrito()
    {
        _itemsPorProducto.Clear();
        _productosSeleccionadosIds.Clear();
    }

    public ResumenCompraDTO GenerarResumen()
    {
        // Materialización de valores del diccionario en List<CarritoItemDTO>
        List<CarritoItemDTO> listaItems = _itemsPorProducto.Values.ToList();

        // Extracción de categorías únicas afectadas mediante HashSet<int>
        HashSet<int> categoriasUnicas = listaItems
            .Select(item => item.IdCategoria)
            .ToHashSet();

        return new ResumenCompraDTO
        {
            Items = listaItems,
            CategoriasAfectadas = categoriasUnicas,
            TotalArticulos = listaItems.Sum(item => item.Cantidad),
            TotalPagar = listaItems.Sum(item => item.Subtotal)
        };
    }
}
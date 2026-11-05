namespace WasiMarketAPI.DTOs;

public record ProductoLecturaDTO(
    int IdProducto,
    int IdTienda,
    int IdCategoria,
    string NombreProducto,
    string? Descripcion,
    decimal PrecioUnitario,
    int StockDisponible,
    int StockMinimo,
    string UnidadMedida,
    string? ImagenUrl,
    bool EstadoActivo
);
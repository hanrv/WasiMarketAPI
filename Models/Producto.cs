namespace WasiMarketAPI.Models;

public class Producto
{
    public int IdProducto { get; set; }
    public int IdTienda { get; set; }
    public int IdCategoria { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int StockDisponible { get; set; }
    public int StockMinimo { get; set; }
    public string UnidadMedida { get; set; } = "UNIDAD";
    public string? ImagenUrl { get; set; }
    public bool EstadoActivo { get; set; } = true;
}
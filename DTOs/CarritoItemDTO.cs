namespace WasiMarketAPI.DTOs;

public class CarritoItemDTO
{
    public int IdProducto { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public int IdCategoria { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;
}
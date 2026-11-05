namespace WasiMarketAPI.DTOs;

public class ResumenCompraDTO
{
    public decimal TotalPagar { get; set; }
    public int TotalArticulos { get; set; }
    public List<CarritoItemDTO> Items { get; set; } = new();
    public HashSet<int> CategoriasAfectadas { get; set; } = new();
}
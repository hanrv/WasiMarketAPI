using System.ComponentModel.DataAnnotations;

namespace WasiMarketAPI.DTOs;

public class AgregarItemCarritoDTO
{
    [Required]
    public int IdProducto { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Cantidad { get; set; }
}
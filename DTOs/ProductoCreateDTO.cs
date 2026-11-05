using System.ComponentModel.DataAnnotations;

namespace WasiMarketAPI.DTOs;

public class ProductoCreateDTO
{
    [Required]
    public int IdTienda { get; set; }

    [Required]
    public int IdCategoria { get; set; }

    [Required]
    [MaxLength(100)]
    public string NombreProducto { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Descripcion { get; set; }

    [Required]
    [Range(0.01, 99999999.99)]
    public decimal PrecioUnitario { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int StockDisponible { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int StockMinimo { get; set; }

    [Required]
    [RegularExpression("^(UNIDAD|KILOGRAMO|LITRO|PAQUETE)$", ErrorMessage = "Unidad no permitida por la base de datos.")]
    public string UnidadMedida { get; set; } = "UNIDAD";

    [MaxLength(255)]
    public string? ImagenUrl { get; set; }
}
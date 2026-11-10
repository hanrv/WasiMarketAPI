using System.ComponentModel.DataAnnotations;

namespace WasiMarketAPI.DTOs
{
    public class ActualizarPrecioDTO
    {
        [Required]
        [Range(0.01, 99999.99, ErrorMessage = "El precio unitario debe ser mayor a 0.")]
        public decimal NuevoPrecio { get; set; }
    }
}
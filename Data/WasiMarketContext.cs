using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WasiMarketAPI.Models;

namespace WasiMarketAPI.Data;

public class WasiMarketContext : DbContext
{
    public WasiMarketContext(DbContextOptions<WasiMarketContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("PRODUCTO");
            entity.HasKey(e => e.IdProducto).HasName("PK_producto");

            entity.Property(e => e.IdProducto).HasColumnName("id_producto").ValueGeneratedOnAdd();
            entity.Property(e => e.IdTienda).HasColumnName("id_tienda").IsRequired();
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria").IsRequired();
            entity.Property(e => e.NombreProducto).HasColumnName("nombre_producto").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(250);
            entity.Property(e => e.PrecioUnitario).HasColumnName("precio_unitario").HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.StockDisponible).HasColumnName("stock_disponible").HasDefaultValue(0);
            entity.Property(e => e.StockMinimo).HasColumnName("stock_minimo").HasDefaultValue(0);
            entity.Property(e => e.UnidadMedida).HasColumnName("unidad_medida").HasMaxLength(20).IsRequired();
            entity.Property(e => e.ImagenUrl).HasColumnName("imagen_url").HasMaxLength(255);
            entity.Property(e => e.EstadoActivo).HasColumnName("estado_activo").HasDefaultValue(true);
        });
    }
}
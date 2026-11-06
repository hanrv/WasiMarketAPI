using Microsoft.EntityFrameworkCore;
using WasiMarketAPI.Models;

namespace WasiMarketAPI.Data;

public class WasiMarketContext : DbContext
{
    public WasiMarketContext(DbContextOptions<WasiMarketContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mapeo de la tabla PRODUCTO
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

        // Mapeo de la tabla USUARIO
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("USUARIO");
            entity.HasKey(e => e.Id).HasName("PK_usuario");

            entity.Property(e => e.Id).HasColumnName("id_usuario").ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100).IsRequired();

            // Cambiar "password_hash" por "contrasena_hash":
            entity.Property(e => e.PasswordHash).HasColumnName("contrasena_hash").HasMaxLength(255).IsRequired();

            entity.Property(e => e.Rol).HasColumnName("rol").HasMaxLength(50).HasDefaultValue("Cliente");
        });
    }
}
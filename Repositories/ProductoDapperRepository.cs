using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using WasiMarketAPI.DTOs;

namespace WasiMarketAPI.Repositories;

public class ProductoDapperRepository : IProductoDapperRepository
{
    private readonly string _connectionString;

    public ProductoDapperRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Cadena de conexión no configurada.");
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<IEnumerable<ProductoLecturaDTO>> ObtenerCatalogoActivoAsync()
    {
        const string sql = @"
            SELECT 
                id_producto AS IdProducto,
                id_tienda AS IdTienda,
                id_categoria AS IdCategoria,
                nombre_producto AS NombreProducto,
                descripcion AS Descripcion,
                precio_unitario AS PrecioUnitario,
                stock_disponible AS StockDisponible,
                stock_minimo AS StockMinimo,
                unidad_medida AS UnidadMedida,
                imagen_url AS ImagenUrl,
                estado_activo AS EstadoActivo
            FROM PRODUCTO
            WHERE estado_activo = 1;";

        using var db = CreateConnection();
        return await db.QueryAsync<ProductoLecturaDTO>(sql);
    }

    public async Task<ProductoLecturaDTO?> ObtenerPorIdAsync(int idProducto)
    {
        const string sql = @"
            SELECT 
                id_producto AS IdProducto,
                id_tienda AS IdTienda,
                id_categoria AS IdCategoria,
                nombre_producto AS NombreProducto,
                descripcion AS Descripcion,
                precio_unitario AS PrecioUnitario,
                stock_disponible AS StockDisponible,
                stock_minimo AS StockMinimo,
                unidad_medida AS UnidadMedida,
                imagen_url AS ImagenUrl,
                estado_activo AS EstadoActivo
            FROM PRODUCTO
            WHERE id_producto = @IdProducto;";

        using var db = CreateConnection();
        return await db.QuerySingleOrDefaultAsync<ProductoLecturaDTO>(sql, new { IdProducto = idProducto });
    }

    public async Task<IEnumerable<ProductoLecturaDTO>> ObtenerPorIdsBatchAsync(IEnumerable<int> ids)
    {
        if (!ids.Any()) return Enumerable.Empty<ProductoLecturaDTO>();

        const string sql = @"
            SELECT 
                id_producto AS IdProducto,
                id_tienda AS IdTienda,
                id_categoria AS IdCategoria,
                nombre_producto AS NombreProducto,
                descripcion AS Descripcion,
                precio_unitario AS PrecioUnitario,
                stock_disponible AS StockDisponible,
                stock_minimo AS StockMinimo,
                unidad_medida AS UnidadMedida,
                imagen_url AS ImagenUrl,
                estado_activo AS EstadoActivo
            FROM PRODUCTO
            WHERE id_producto IN @Ids AND estado_activo = 1;";

        using var db = CreateConnection();
        return await db.QueryAsync<ProductoLecturaDTO>(sql, new { Ids = ids });
    }
}
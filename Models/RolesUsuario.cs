namespace WasiMarketAPI.Models;

public static class RolesUsuario
{
    public const string Administrador = "Administrador";
    public const string Cliente = "Cliente";
    public const string Bodeguero = "Bodeguero";

    public static readonly string[] Todos = [Administrador, Cliente, Bodeguero];
}
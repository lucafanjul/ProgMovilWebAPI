
namespace ProgMovilWebAPI.Models;

public class Venta
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public decimal Total { get; set; }

    public List<DetalleVenta> Detalles { get; set; } = new();
}

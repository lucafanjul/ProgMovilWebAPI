
namespace ProgMovilWebAPI.Models
{
    public class SalidaProducto
    {
        public int Id { get; set; }

        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        // Usuario que registró la salida
        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int Cantidad { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}

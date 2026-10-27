
using ProgMovilWebAPI.Models;

namespace ProgMovilWebAPI.DTOs
{
    public class SalidaProductoDTO
    {
        public int Id { get; set; }

        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public int Cantidad { get; set; }

        public DateTime Fecha { get; set; }

        public int? UsuarioId { get; set; }

        public UsuarioResumenDTO? Usuario { get; set; }
    }
}

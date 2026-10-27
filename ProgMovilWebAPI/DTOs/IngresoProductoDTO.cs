
using ProgMovilWebAPI.Models;

namespace ProgMovilWebAPI.DTOs
{
    public class IngresoProductoDTO
    {
        public int Id { get; set; }

        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }

        public int ProveedorId { get; set; }
        public Proveedor? Proveedor { get; set; }

        public int Cantidad { get; set; }

        public DateTime Fecha { get; set; }

        public int? UsuarioId { get; set; }

        public UsuarioResumenDTO? Usuario { get; set; }
    }
}

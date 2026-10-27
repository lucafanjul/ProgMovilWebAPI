
using System.ComponentModel.DataAnnotations;

namespace ProgMovilWebAPI.DTOs
{
    public class CrearIngresoProductoDTO
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "Debe seleccionar un producto válido.")]
        public int ProductoId { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Debe seleccionar un proveedor válido.")]
        public int ProveedorId { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Cantidad { get; set; }
    }
}

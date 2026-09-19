namespace ProgMovilWebAPI.Models
{
    public class IngresoProducto
    {
        public int Id { get; set; }

        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }

        public int ProveedorId { get; set; }
        public Proveedor? Proveedor { get; set; }

        public int Cantidad { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}
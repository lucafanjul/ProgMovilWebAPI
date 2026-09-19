namespace ProgMovilWebAPI.Models
{
    public class SalidaProducto
    {
        public int Id { get; set; }

        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public int Cantidad { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}
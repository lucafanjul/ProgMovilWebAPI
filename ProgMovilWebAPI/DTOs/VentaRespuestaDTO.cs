
namespace ProgMovilWebAPI.DTOs
{
    public class VentaRespuestaDTO
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Total { get; set; }

        public List<DetalleVentaRespuestaDTO> Detalles { get; set; }
            = new();
    }

    public class DetalleVentaRespuestaDTO
    {
        public int ProductoId { get; set; }

        public string NombreProducto { get; set; }
            = string.Empty;

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }
    }
}

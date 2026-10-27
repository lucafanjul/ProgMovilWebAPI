namespace ProgMovilWebAPI.Models
{
    public class Producto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public int Stock { get; set; }

        public decimal Precio { get; set; }

        public int CategoriaId { get; set; }

        public Categoria? Categoria { get; set; }

        // Ruta de la imagen principal del producto
        public string? ImagenUrl { get; set; }

        // Relación existente con la tabla Imagenes
        public List<Imagen> Imagenes { get; set; } = new List<Imagen>();
    }
}
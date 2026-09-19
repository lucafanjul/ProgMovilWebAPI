namespace ProgMovilWebAPI.Models
{
    public class Imagen
    {
        public int Id { get; set; }

        public string NombreOriginal { get; set; } = string.Empty;

        public string NombreArchivo { get; set; } = string.Empty;

        public string RutaRelativa { get; set; } = string.Empty;

        public string TipoContenido { get; set; } = string.Empty;

        public long TamanoBytes { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public int ProductoId { get; set; }

        public Producto? Producto { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace ProgMovilWebAPI.DTOs
{
    public class ActualizarMiClienteDTO
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(30)]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(200)]
        public string Direccion { get; set; } = string.Empty;
    }
}
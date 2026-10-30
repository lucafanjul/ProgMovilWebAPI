
using System.ComponentModel.DataAnnotations;

namespace ProgMovilWebAPI.DTOs;

public class CrearVentaDTO
{
    [Required, MinLength(1)]
    public List<CrearDetalleVentaDTO> Detalles { get; set; } = new();
}

public class CrearDetalleVentaDTO
{
    [Range(1, int.MaxValue)]
    public int ProductoId { get; set; }

    [Range(1, int.MaxValue)]
    public int Cantidad { get; set; }
}

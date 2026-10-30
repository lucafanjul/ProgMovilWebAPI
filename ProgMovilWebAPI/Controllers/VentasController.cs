
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgMovilWebAPI.Data;
using ProgMovilWebAPI.DTOs;
using ProgMovilWebAPI.Models;
using System.Data;
using System.Security.Claims;

namespace ProgMovilWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VentasController : ControllerBase
    {
        private readonly DataContext _context;

        public VentasController(DataContext context)
        {
            _context = context;
        }

        private int? ObtenerUsuarioId()
        {
            var valor = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(valor, out int id)
                ? id : null;
        }

        private static VentaRespuestaDTO Convertir(Venta venta)
        {
            return new VentaRespuestaDTO
            {
                Id = venta.Id,
                Fecha = venta.Fecha,
                Total = venta.Total,
                Detalles = venta.Detalles.Select(d =>
                    new DetalleVentaRespuestaDTO
                    {
                        ProductoId = d.ProductoId,
                        NombreProducto = d.Producto?.Nombre
                            ?? "Producto",
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList()
            };
        }

        [HttpPost]
        public async Task<IActionResult> Crear(
            [FromBody] CrearVentaDTO dto)
        {
            var usuarioId = ObtenerUsuarioId();

            if (usuarioId == null)
                return Unauthorized();

            if (dto.Detalles == null || dto.Detalles.Count == 0)
                return BadRequest("El carrito está vacío.");

            if (dto.Detalles.Any(d =>
                d.ProductoId <= 0 || d.Cantidad <= 0))
            {
                return BadRequest(
                    "Los productos y cantidades deben ser válidos.");
            }

            var detalles = new List<CrearDetalleVentaDTO>();

            try
            {
                detalles = dto.Detalles
                    .GroupBy(d => d.ProductoId)
                    .Select(g => new CrearDetalleVentaDTO
                    {
                        ProductoId = g.Key,
                        Cantidad = checked(g.Sum(x => x.Cantidad))
                    }).ToList();
            }
            catch (OverflowException)
            {
                return BadRequest(
                    "La cantidad solicitada es demasiado grande.");
            }

            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.ReadCommitted);

            try
            {
                var ids = detalles.Select(d => d.ProductoId)
                    .ToList();

                var productos = await _context.Productos
                    .Where(p => ids.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);

                if (productos.Count != detalles.Count)
                {
                    await transaccion.RollbackAsync();
                    return BadRequest(
                        "Uno o más productos no existen.");
                }

                var venta = new Venta
                {
                    UsuarioId = usuarioId.Value,
                    Fecha = DateTime.UtcNow
                };

                foreach (var item in detalles.OrderBy(d => d.ProductoId))
                {
                    var producto = productos[item.ProductoId];

                    if (producto.Precio < 0)
                    {
                        await transaccion.RollbackAsync();
                        return BadRequest(
                            "El producto tiene un precio inválido.");
                    }

                    // Actualización atómica: evita vender
                    // más unidades que las disponibles.
                    var actualizados = await _context.Productos
                        .Where(p => p.Id == item.ProductoId
                            && p.Stock >= item.Cantidad)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(
                                p => p.Stock,
                                p => p.Stock - item.Cantidad));

                    if (actualizados != 1)
                    {
                        await transaccion.RollbackAsync();

                        return Conflict(
                            $"Stock insuficiente para {producto.Nombre}.");
                    }

                    var subtotal = checked(
                        producto.Precio * item.Cantidad);

                    venta.Detalles.Add(new DetalleVenta
                    {
                        ProductoId = producto.Id,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = producto.Precio,
                        Subtotal = subtotal
                    });
                }

                venta.Total = checked(
                    venta.Detalles.Sum(d => d.Subtotal));

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                await transaccion.CommitAsync();

                // Se vuelve a consultar para obtener
                // los nombres y los detalles guardados.
                var guardada = await _context.Ventas
                    .AsNoTracking()
                    .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                    .FirstAsync(v => v.Id == venta.Id);

                return CreatedAtAction(
                    nameof(ObtenerPorId),
                    new { id = venta.Id },
                    Convertir(guardada));
            }
            catch (OverflowException)
            {
                await transaccion.RollbackAsync();
                return BadRequest(
                    "El importe total excede el límite permitido.");
            }
            catch (Exception)
            {
                await transaccion.RollbackAsync();
                return StatusCode(500,
                    "No se pudo completar la compra.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            var usuarioId = ObtenerUsuarioId();

            if (usuarioId == null)
                return Unauthorized();

            var consulta = _context.Ventas
                .AsNoTracking()
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                consulta = consulta.Where(
                    v => v.UsuarioId == usuarioId.Value);
            }

            var ventas = await consulta
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();

            return Ok(ventas.Select(Convertir));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var usuarioId = ObtenerUsuarioId();

            if (usuarioId == null)
                return Unauthorized();

            var consulta = _context.Ventas
                .AsNoTracking()
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .Where(v => v.Id == id);

            if (!User.IsInRole("Admin"))
            {
                consulta = consulta.Where(
                    v => v.UsuarioId == usuarioId.Value);
            }

            var venta = await consulta.FirstOrDefaultAsync();

            if (venta == null)
                return NotFound("Venta no encontrada.");

            return Ok(Convertir(venta));
        }
    }
}

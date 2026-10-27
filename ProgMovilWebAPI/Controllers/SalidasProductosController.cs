using ProgMovilWebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgMovilWebAPI.Data;
using ProgMovilWebAPI.Models;
using System.Security.Claims;

namespace ProgMovilWebAPI.Controllers
{
    [Authorize(Roles = "Admin,Usuario")]
    [Route("api/[controller]")]
    [ApiController]
    public class SalidasProductosController : ControllerBase
    {
        private readonly DataContext _context;

        public SalidasProductosController(DataContext context)
        {
            _context = context;
        }
        private static SalidaProductoDTO ConvertirADTO(SalidaProducto salida)
        {
            return new SalidaProductoDTO
            {
                Id = salida.Id,
                ProductoId = salida.ProductoId,
                Producto = salida.Producto,
                ClienteId = salida.ClienteId,
                Cliente = salida.Cliente,
                Cantidad = salida.Cantidad,
                Fecha = salida.Fecha,
                UsuarioId = salida.UsuarioId,
                Usuario = salida.Usuario == null
                    ? null
                    : new UsuarioResumenDTO
                    {
                        Id = salida.Usuario.Id,
                        Username = salida.Usuario.Username
                    }
            };
        }
        // GET: api/SalidasProductos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalidaProductoDTO>>> GetSalidas()
        {
            var salidas = await _context.SalidasProductos
                .AsNoTracking()
                .Include(s => s.Producto)
                .Include(s => s.Cliente)
                .Include(s => s.Usuario)
                .OrderByDescending(s => s.Fecha)
                .ToListAsync();

            return Ok(salidas.Select(ConvertirADTO).ToList());
        }

        // GET: api/SalidasProductos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SalidaProductoDTO>> GetSalida(int id)
        {
            var salida = await _context.SalidasProductos
                .AsNoTracking()
                .Include(s => s.Producto)
                .Include(s => s.Cliente)
                .Include(s => s.Usuario)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (salida == null)
            {
                return NotFound();
            }

            return Ok(ConvertirADTO(salida));
        }

        // GET: api/SalidasProductos/paginado?pageNumber=1&pageSize=10
        [HttpGet("paginado")]
        public async Task<ActionResult> GetSalidasPaginadas(
            int pageNumber = 1,
            int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest(
                    "pageNumber y pageSize deben ser mayores a 0.");
            }

            var totalRegistros =
                await _context.SalidasProductos.CountAsync();

            var salidas = await _context.SalidasProductos
                .AsNoTracking()
                .Include(s => s.Producto)
                .Include(s => s.Cliente)
                .Include(s => s.Usuario)
                .OrderByDescending(s => s.Fecha)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRegistros = totalRegistros,
                TotalPaginas = (int)Math.Ceiling(
                    totalRegistros / (double)pageSize),
                Datos = salidas.Select(ConvertirADTO).ToList()
            });
        }

        // POST: api/SalidasProductos

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<SalidaProductoDTO>> PostSalida(
            [FromBody] CrearSalidaProductoDTO dto)
        {
            // Obtener el ID del usuario autenticado desde el JWT
            var usuarioIdTexto =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(usuarioIdTexto, out int usuarioId))
            {
                return Unauthorized(
                    "No se pudo identificar al usuario autenticado.");
            }

            // Validar la cantidad solicitada
            if (dto.Cantidad <= 0)
            {
                return BadRequest(
                    "La cantidad vendida debe ser mayor a 0.");
            }

            // Verificar que el producto exista
            var producto = await _context.Productos
                .FindAsync(dto.ProductoId);

            if (producto == null)
            {
                return BadRequest("El producto no existe.");
            }

            // Verificar que el cliente exista
            var cliente = await _context.Clientes
                .FindAsync(dto.ClienteId);

            if (cliente == null)
            {
                return BadRequest("El cliente no existe.");
            }

            // Verificar que exista stock suficiente
            if (producto.Stock < dto.Cantidad)
            {
                return BadRequest(
                    $"Stock insuficiente. Stock disponible: {producto.Stock}.");
            }

            // Crear la salida con datos controlados por el servidor
            var salida = new SalidaProducto
            {
                ProductoId = dto.ProductoId,
                ClienteId = dto.ClienteId,
                Cantidad = dto.Cantidad,
                Fecha = DateTime.UtcNow,
                UsuarioId = usuarioId
            };

            // Descontar la mercadería del stock
            producto.Stock -= dto.Cantidad;

            // Registrar la salida
            _context.SalidasProductos.Add(salida);

            await _context.SaveChangesAsync();

            // Cargar las relaciones para la respuesta
            salida.Producto = producto;
            salida.Cliente = cliente;

            salida.Usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            // Devolver el DTO sin información sensible del usuario
            return CreatedAtAction(
                nameof(GetSalida),
                new { id = salida.Id },
                ConvertirADTO(salida));
        }

    }
}

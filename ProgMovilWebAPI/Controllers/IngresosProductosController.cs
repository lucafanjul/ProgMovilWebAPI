using ProgMovilWebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgMovilWebAPI.Data;
using ProgMovilWebAPI.Models;
using System.Security.Claims;

namespace ProgMovilWebAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class IngresosProductosController : ControllerBase
    {
        private readonly DataContext _context;

        public IngresosProductosController(DataContext context)
        {
            _context = context;
        }

        private static IngresoProductoDTO ConvertirADTO(IngresoProducto ingreso)
        {
            return new IngresoProductoDTO
            {
                Id = ingreso.Id,
                ProductoId = ingreso.ProductoId,
                Producto = ingreso.Producto,
                ProveedorId = ingreso.ProveedorId,
                Proveedor = ingreso.Proveedor,
                Cantidad = ingreso.Cantidad,
                Fecha = ingreso.Fecha,
                UsuarioId = ingreso.UsuarioId,
                Usuario = ingreso.Usuario == null
                    ? null
                    : new UsuarioResumenDTO
                    {
                        Id = ingreso.Usuario.Id,
                        Username = ingreso.Usuario.Username
                    }
            };
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<IngresoProductoDTO>>> GetIngresos()
        {
            var ingresos = await _context.IngresosProductos
                .AsNoTracking()
                .Include(i => i.Producto)
                .Include(i => i.Proveedor)
                .Include(i => i.Usuario)
                .OrderByDescending(i => i.Fecha)
                .ToListAsync();

            return Ok(ingresos.Select(ConvertirADTO).ToList());
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<IngresoProductoDTO>> GetIngreso(int id)
        {
            var ingreso = await _context.IngresosProductos
                .AsNoTracking()
                .Include(i => i.Producto)
                .Include(i => i.Proveedor)
                .Include(i => i.Usuario)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (ingreso == null)
            {
                return NotFound();
            }

            return Ok(ConvertirADTO(ingreso));
        }


        // GET: api/IngresosProductos/paginado?pageNumber=1&pageSize=10

        [HttpGet("paginado")]
        public async Task<ActionResult> GetIngresosPaginados(
            int pageNumber = 1,
            int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest(
                    "pageNumber y pageSize deben ser mayores a 0.");
            }

            var totalRegistros =
                await _context.IngresosProductos.CountAsync();

            var ingresos = await _context.IngresosProductos
                .AsNoTracking()
                .Include(i => i.Producto)
                .Include(i => i.Proveedor)
                .Include(i => i.Usuario)
                .OrderByDescending(i => i.Fecha)
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
                Datos = ingresos.Select(ConvertirADTO).ToList()
            });
        }


        // POST: api/IngresosProductos

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<IngresoProductoDTO>> PostIngreso(
            [FromBody] CrearIngresoProductoDTO dto)
        {
            // Obtener el ID del usuario autenticado desde el JWT
            var usuarioIdTexto =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(usuarioIdTexto, out int usuarioId))
            {
                return Unauthorized(
                    "No se pudo identificar al usuario autenticado.");
            }

            if (dto.Cantidad <= 0)
            {
                return BadRequest(
                    "La cantidad ingresada debe ser mayor a 0.");
            }

            // Verificar que el producto exista
            var producto = await _context.Productos
                .FindAsync(dto.ProductoId);

            if (producto == null)
            {
                return BadRequest("El producto no existe.");
            }

            // Verificar que el proveedor exista
            var proveedor = await _context.Proveedores
                .FindAsync(dto.ProveedorId);

            if (proveedor == null)
            {
                return BadRequest("El proveedor no existe.");
            }

            // Crear el ingreso con datos controlados por el servidor
            var ingreso = new IngresoProducto
            {
                ProductoId = dto.ProductoId,
                ProveedorId = dto.ProveedorId,
                Cantidad = dto.Cantidad,
                Fecha = DateTime.UtcNow,
                UsuarioId = usuarioId
            };

            // Aumentar el stock del producto
            producto.Stock += dto.Cantidad;

            // Registrar el movimiento
            _context.IngresosProductos.Add(ingreso);

            await _context.SaveChangesAsync();

            // Cargar las relaciones para devolver el DTO completo
            ingreso.Producto = producto;
            ingreso.Proveedor = proveedor;

            ingreso.Usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            return CreatedAtAction(
                nameof(GetIngreso),
                new { id = ingreso.Id },
                ConvertirADTO(ingreso));
        }

    }
}

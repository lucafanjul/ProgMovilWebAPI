using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgMovilWebAPI.Data;
using ProgMovilWebAPI.Models;

namespace ProgMovilWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngresosProductosController : ControllerBase
    {
        private readonly DataContext _context;

        public IngresosProductosController(DataContext context)
        {
            _context = context;
        }

        // GET: api/IngresosProductos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IngresoProducto>>> GetIngresos()
        {
            return await _context.IngresosProductos
                .AsNoTracking()
                .Include(i => i.Producto)
                .Include(i => i.Proveedor)
                .OrderByDescending(i => i.Fecha)
                .ToListAsync();
        }

        // GET: api/IngresosProductos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IngresoProducto>> GetIngreso(int id)
        {
            var ingreso = await _context.IngresosProductos
                .AsNoTracking()
                .Include(i => i.Producto)
                .Include(i => i.Proveedor)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (ingreso == null)
            {
                return NotFound();
            }

            return ingreso;
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
                Datos = ingresos
            });
        }

        // POST: api/IngresosProductos
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<IngresoProducto>> PostIngreso(
            IngresoProducto ingreso)
        {
            if (ingreso.Cantidad <= 0)
            {
                return BadRequest(
                    "La cantidad ingresada debe ser mayor a 0.");
            }

            var producto = await _context.Productos
                .FindAsync(ingreso.ProductoId);

            if (producto == null)
            {
                return BadRequest("El producto no existe.");
            }

            var proveedor = await _context.Proveedores
                .FindAsync(ingreso.ProveedorId);

            if (proveedor == null)
            {
                return BadRequest("El proveedor no existe.");
            }

            ingreso.Id = 0;
            ingreso.Fecha = DateTime.UtcNow;

            // El ingreso de mercadería aumenta el stock.
            producto.Stock += ingreso.Cantidad;

            _context.IngresosProductos.Add(ingreso);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetIngreso),
                new { id = ingreso.Id },
                ingreso);
        }
    }
}
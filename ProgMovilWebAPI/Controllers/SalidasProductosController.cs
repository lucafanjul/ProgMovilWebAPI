using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgMovilWebAPI.Data;
using ProgMovilWebAPI.Models;

namespace ProgMovilWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalidasProductosController : ControllerBase
    {
        private readonly DataContext _context;

        public SalidasProductosController(DataContext context)
        {
            _context = context;
        }

        // GET: api/SalidasProductos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalidaProducto>>> GetSalidas()
        {
            return await _context.SalidasProductos
                .AsNoTracking()
                .Include(s => s.Producto)
                .Include(s => s.Cliente)
                .OrderByDescending(s => s.Fecha)
                .ToListAsync();
        }

        // GET: api/SalidasProductos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SalidaProducto>> GetSalida(int id)
        {
            var salida = await _context.SalidasProductos
                .AsNoTracking()
                .Include(s => s.Producto)
                .Include(s => s.Cliente)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (salida == null)
            {
                return NotFound();
            }

            return salida;
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
                Datos = salidas
            });
        }

        // POST: api/SalidasProductos
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<SalidaProducto>> PostSalida(
            SalidaProducto salida)
        {
            if (salida.Cantidad <= 0)
            {
                return BadRequest(
                    "La cantidad vendida debe ser mayor a 0.");
            }

            var producto = await _context.Productos
                .FindAsync(salida.ProductoId);

            if (producto == null)
            {
                return BadRequest("El producto no existe.");
            }

            var cliente = await _context.Clientes
                .FindAsync(salida.ClienteId);

            if (cliente == null)
            {
                return BadRequest("El cliente no existe.");
            }

            if (producto.Stock < salida.Cantidad)
            {
                return BadRequest(
                    $"Stock insuficiente. Stock disponible: {producto.Stock}.");
            }

            salida.Id = 0;
            salida.Fecha = DateTime.UtcNow;

            // La venta disminuye el stock.
            producto.Stock -= salida.Cantidad;

            _context.SalidasProductos.Add(salida);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetSalida),
                new { id = salida.Id },
                salida);
        }
    }
}
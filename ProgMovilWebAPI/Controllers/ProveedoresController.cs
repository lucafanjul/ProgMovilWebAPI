using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgMovilWebAPI.Data;
using ProgMovilWebAPI.Models;

namespace ProgMovilWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly DataContext _context;

        public ProveedoresController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Proveedores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetProveedores()
        {
            return await _context.Proveedores
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/Proveedores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Proveedor>> GetProveedor(int id)
        {
            var proveedor = await _context.Proveedores
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proveedor == null)
            {
                return NotFound();
            }

            return proveedor;
        }

        // GET: api/Proveedores/paginado?pageNumber=1&pageSize=10
        [HttpGet("paginado")]
        public async Task<ActionResult> GetProveedoresPaginados(
            int pageNumber = 1,
            int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest(
                    "pageNumber y pageSize deben ser mayores a 0.");
            }

            var totalRegistros = await _context.Proveedores.CountAsync();

            var proveedores = await _context.Proveedores
                .AsNoTracking()
                .OrderBy(p => p.Id)
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
                Datos = proveedores
            });
        }

        // POST: api/Proveedores
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Proveedor>> PostProveedor(
            Proveedor proveedor)
        {
            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProveedor),
                new { id = proveedor.Id },
                proveedor);
        }

        // PUT: api/Proveedores/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProveedor(
            int id,
            Proveedor proveedor)
        {
            if (id != proveedor.Id)
            {
                return BadRequest();
            }

            var existe = await _context.Proveedores
                .AnyAsync(p => p.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            _context.Entry(proveedor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Proveedores/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
            {
                return NotFound();
            }

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
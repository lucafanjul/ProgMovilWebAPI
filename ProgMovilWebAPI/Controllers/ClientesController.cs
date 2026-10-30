using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgMovilWebAPI.Data;
using ProgMovilWebAPI.Models;
using System.Security.Claims;
using ProgMovilWebAPI.DTOs;
namespace ProgMovilWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ClientesController : ControllerBase
    {
        private readonly DataContext _context;

        public ClientesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        [Authorize(Roles = "Admin,Usuario")]
        public async Task<IActionResult> ObtenerMiPerfil()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(claim, out int usuarioId))
                return Unauthorized();

            var cliente = await _context.Clientes
                .AsNoTracking()
                .Where(c => c.UsuarioId == usuarioId)
                .Select(c => new
                {
                    c.Id,
                    c.Nombre,
                    c.Email,
                    c.Telefono,
                    c.Direccion
                })
                .FirstOrDefaultAsync();

            if (cliente == null)
                return NotFound("No existe un perfil de cliente asociado a este usuario.");

            return Ok(cliente);
        }

        [HttpPut("me")]
        [Authorize(Roles = "Admin,Usuario")]
        public async Task<IActionResult> ActualizarMiPerfil(
            [FromBody] ActualizarMiClienteDTO dto)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(claim, out int usuarioId))
                return Unauthorized();

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (cliente == null)
                return NotFound("No existe un perfil de cliente asociado a este usuario.");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre es obligatorio.");

            cliente.Nombre = dto.Nombre.Trim();
            cliente.Telefono = dto.Telefono?.Trim() ?? string.Empty;
            cliente.Direccion = dto.Direccion?.Trim() ?? string.Empty;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Perfil actualizado correctamente.",
                cliente.Id,
                cliente.Nombre,
                cliente.Email,
                cliente.Telefono,
                cliente.Direccion
            });
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            return await _context.Clientes
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        // GET: api/Clientes/paginado?pageNumber=1&pageSize=10
        [HttpGet("paginado")]
        public async Task<ActionResult> GetClientesPaginados(
            int pageNumber = 1,
            int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest(
                    "pageNumber y pageSize deben ser mayores a 0.");
            }

            var totalRegistros = await _context.Clientes.CountAsync();

            var clientes = await _context.Clientes
                .AsNoTracking()
                .OrderBy(c => c.Id)
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
                Datos = clientes
            });
        }

        // POST: api/Clientes
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCliente),
                new { id = cliente.Id },
                cliente);
        }

        // PUT: api/Clientes/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(
            int id,
            Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return BadRequest();
            }

            var existe = await _context.Clientes
                .AnyAsync(c => c.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            _context.Entry(cliente).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Clientes/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
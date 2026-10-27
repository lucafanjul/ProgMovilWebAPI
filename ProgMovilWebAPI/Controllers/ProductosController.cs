using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgMovilWebAPI.Data;
using ProgMovilWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
namespace ProgMovilWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Usuario")]
    public class ProductosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        
        private readonly string[] _extensionesPermitidas =
            [".jpg", ".jpeg", ".png"];

        private readonly string[] _tiposMimePermitidos =
            ["image/jpeg", "image/png"];

        private const long MaxFileSize = 5 * 1024 * 1024;


        public ProductosController(
            DataContext context,
            IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        // =========================================================
        // GET: api/Productos
        // Lista todos los productos
        // =========================================================

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagenes)
                .ToListAsync();
        }


        // =========================================================
        // GET: api/Productos/paginado
        // Lista productos utilizando paginación
        // =========================================================

        [HttpGet("paginado")]
        public async Task<ActionResult> GetProductosPaginados(
            int pageNumber = 1,
            int pageSize = 10)
        {
            if (pageNumber < 1)
            {
                return BadRequest(new
                {
                    mensaje = "El número de página debe ser mayor o igual a 1."
                });
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new
                {
                    mensaje = "El tamaño de página debe estar entre 1 y 100."
                });
            }
            var query = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagenes)
                .AsQueryable();

            var totalRegistros = await query.CountAsync();

            var productos = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                pageNumber,
                pageSize,
                totalRegistros,

                totalPaginas = (int)Math.Ceiling(
                    totalRegistros / (double)pageSize),

                productos
            });
        }


        // =========================================================
        // GET: api/Productos/5
        // Busca un producto por ID
        // =========================================================

        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagenes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            return Ok(producto);
        }


        // =========================================================
        // POST: api/Productos
        // Crea un producto
        // =========================================================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(
            Producto producto)
        {
            var categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.Id == producto.CategoriaId);

            if (!categoriaExiste)
            {
                return BadRequest(
                    "La categoría indicada no existe.");
            }

            producto.Categoria = null;

            // Un producto nuevo comienza sin movimientos de stock.
            // El stock se incrementa mediante ingresos de productos.
            producto.Stock = 0;

            // La imagen se asigna mediante el endpoint de carga.
            producto.ImagenUrl = null;

            // Evitar asociar imágenes directamente desde el JSON.
            producto.Imagenes = new List<Imagen>();

            _context.Productos.Add(producto);

            await _context.SaveChangesAsync();

            return Ok(producto);
        }


        // =========================================================
        // PUT: api/Productos/5
        // Modifica un producto
        // =========================================================
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(
            int id,
            Producto producto)
        {
            if (id != producto.Id)
            {
                return BadRequest();
            }

            var categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.Id == producto.CategoriaId);

            if (!categoriaExiste)
            {
                return BadRequest(
                    "La categoría indicada no existe.");
            }

            producto.Categoria = null;

            // Buscar el producto existente
            var productoExistente = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productoExistente == null)
            {
                return NotFound();
            }

            // Actualizar únicamente los datos editables
            productoExistente.Nombre = producto.Nombre;
            productoExistente.Precio = producto.Precio;
            productoExistente.CategoriaId = producto.CategoriaId;

            // No modificamos ImagenUrl ni Stock.
            // ImagenUrl se actualiza al subir imágenes.
            // Stock se actualiza mediante ingresos y salidas.

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExiste(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }


        // =========================================================
        // DELETE: api/Productos/5
        // Elimina un producto
        // =========================================================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto =
                await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);

            await _context.SaveChangesAsync();

            return NoContent();
        }


        // =========================================================
        // POST: api/Productos/5/imagenes
        // Sube una imagen para un producto
        // =========================================================
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/imagenes")]
        public async Task<ActionResult> SubirImagen(
    int id,
    IFormFile archivo)
        {
            // 1. Buscar el producto

            var producto =
                await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound(new
                {
                    mensaje =
                        $"El producto con ID {id} no existe."
                });
            }


            // 2. Verificar que el producto no tenga ya una imagen

            var yaTieneImagen = await _context.Imagenes
                .AnyAsync(i => i.ProductoId == id);

            if (yaTieneImagen || !string.IsNullOrWhiteSpace(producto.ImagenUrl))
            {
                return BadRequest(new
                {
                    mensaje =
                        "El producto ya tiene una imagen asociada."
                });
            }

            // 3. Comprobar que se haya enviado un archivo

            if (archivo == null || archivo.Length == 0)
            {
                return BadRequest(new
                {
                    mensaje =
                        "Debe proporcionar una imagen válida."
                });
            }


            // 4. Validar tamaño máximo de 5 MB

            if (archivo.Length > MaxFileSize)
            {
                return BadRequest(new
                {
                    mensaje =
                        "La imagen supera el máximo permitido de 5 MB."
                });
            }


            // 5. Obtener extensión

            var extension = Path
                .GetExtension(archivo.FileName)
                .ToLowerInvariant();


            // 6. Validar extensión

            if (!_extensionesPermitidas.Contains(extension))
            {
                return BadRequest(new
                {
                    mensaje =
                        "Solo se permiten archivos JPG, JPEG o PNG."
                });
            }


            // 7. Validar tipo MIME

            if (!_tiposMimePermitidos.Contains(
                    archivo.ContentType.ToLowerInvariant()))
            {
                return BadRequest(new
                {
                    mensaje =
                        "Tipo de archivo no válido."
                });
            }


            // 8. Obtener la carpeta wwwroot

            var webRoot = _env.WebRootPath
                ?? Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot");


            // 9. Crear carpeta uploads

            var uploadsDir =
                Path.Combine(webRoot, "uploads");

            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }


            // 10. Generar un nombre único

            var nombreUnico =
                $"{Guid.NewGuid()}{extension}";


            // 11. Crear ruta física del archivo

            var rutaFisica =
                Path.Combine(uploadsDir, nombreUnico);


            // 12. Guardar imagen en disco

            using (var stream =
                   new FileStream(
                       rutaFisica,
                       FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }


            // 13. Crear registro de Imagen

            var imagen = new Imagen
            {
                NombreOriginal =
                    Path.GetFileName(archivo.FileName),

                NombreArchivo =
                    nombreUnico,

                RutaRelativa =
                    $"uploads/{nombreUnico}",

                TipoContenido =
                    archivo.ContentType,

                TamanoBytes =
                    archivo.Length,

                ProductoId =
                    id
            };


            // 14. Guardar información en SQL Server
            _context.Imagenes.Add(imagen);

            producto.ImagenUrl = imagen.RutaRelativa;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Si falla SQL Server, eliminar el archivo físico
                if (System.IO.File.Exists(rutaFisica))
                {
                    System.IO.File.Delete(rutaFisica);
                }

                throw;
            }

            // 15. Crear URL pública de la imagen

            var baseUrl =
                $"{Request.Scheme}://{Request.Host}";


            // 16. Devolver datos de la imagen

            return Ok(new
            {
                imagen.Id,

                imagen.NombreOriginal,

                url =
                    $"{baseUrl}/{imagen.RutaRelativa}",

                imagen.TipoContenido,

                imagen.TamanoBytes,

                imagen.FechaCreacion
            });
        }


        // =========================================================
        // Método auxiliar
        // =========================================================

        private bool ProductoExiste(int id)
        {
            return _context.Productos
                .Any(p => p.Id == id);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgMovilWebAPI.Data;
using ProgMovilWebAPI.DTOs;
using ProgMovilWebAPI.Models;
using ProgMovilWebAPI.Services;

namespace ProgMovilWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly JwtService _jwtService;

        public AuthController(
            DataContext context,
            JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }


        [HttpPost("registro")]
        public async Task<ActionResult<AuthResponseDto>> Registro(
            RegistroDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Todos los campos son obligatorios.");
            }

            var existeUsuario = await _context.Usuarios
                .AnyAsync(u => u.Username == dto.Username);

            if (existeUsuario)
            {
                return BadRequest("El nombre de usuario ya existe.");
            }

            await using var transaccion =
                await _context.Database.BeginTransactionAsync();

            var usuario = new Usuario
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = "Usuario"
            };

            _context.Usuarios.Add(usuario);

            var cliente = new Cliente
            {
                Nombre = dto.Username,
                Email = dto.Email,
                Telefono = string.Empty,
                Direccion = string.Empty,
                Usuario = usuario
            };

            _context.Clientes.Add(cliente);

            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            var (token, expiracion) =
                _jwtService.GenerarToken(usuario);

            return Ok(new AuthResponseDto(
                token,
                usuario.Username,
                usuario.Rol,
                expiracion
            ));
        }



        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(
            LoginDto dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(
                    u => u.Username == dto.Username);

            if (usuario == null)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            var passwordCorrecta =
                BCrypt.Net.BCrypt.Verify(
                    dto.Password,
                    usuario.PasswordHash
                );

            if (!passwordCorrecta)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            var (token, expiracion) =
                _jwtService.GenerarToken(usuario);

            return Ok(new AuthResponseDto(
                token,
                usuario.Username,
                usuario.Rol,
                expiracion
            ));
        }
    }
}
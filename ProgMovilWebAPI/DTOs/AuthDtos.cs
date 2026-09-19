namespace ProgMovilWebAPI.DTOs
{
    public record RegistroDto(
        string Username,
        string Email,
        string Password,
        string Rol = "Usuario"
    );

    public record LoginDto(
        string Username,
        string Password
    );

    public record AuthResponseDto(
        string Token,
        string Username,
        string Rol,
        DateTime Expiracion
    );
}
namespace ProgMovilWebAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public Cliente? Cliente { get; set; }

        public string PasswordHash { get; set; } = string.Empty;

        public string Rol { get; set; } = "Usuario";
    }
}
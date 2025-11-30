namespace TravelRequests.Application.DTOs
{
    // DTO registro
    public class RegistrarUsuarioDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }

    // DTO login
    public class LoginUsuarioDto
    {
        public string Correo { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
    }

    // DTO respuesta usuario
    public class UsuarioResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string FechaCreacion { get; set; } = string.Empty;
    }

    // DTO solicitar código
    public class OlvidarContraseñaDto
    {
        public string Correo { get; set; } = string.Empty;
    }

    // DTO restablecer contraseña
    public class RestablecerContraseñaDto
    {
        public string Correo { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string NuevaContraseña { get; set; } = string.Empty;
    }
}

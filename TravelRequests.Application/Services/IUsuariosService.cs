using TravelRequests.Application.DTOs;

namespace TravelRequests.Application.Servicies;

// Contrato servicio usuarios
public interface IUsuariosService
{
    Task<UsuarioResponseDto> RegistrarUsuarioAsync(RegistrarUsuarioDto registrarDto);
    Task<string> LoginAsync(LoginUsuarioDto loginDto);
    Task<string> OlvidarContraseñaAsync(OlvidarContraseñaDto olvidarDto);
    Task RestablecerContraseñaAsync(RestablecerContraseñaDto restablecerDto);
    Task<List<UsuarioResponseDto>> ObtenerTodosUsuariosAsync(string rolUsuarioActual);
}

using TravelRequests.Domain.Entities;

namespace TravelRequests.Domain.Interfaces;

public interface IRepositorioUsuario
{
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<Usuario?> ObtenerPorCorreoAsync(string correo);
    Task<List<Usuario>> ObtenerTodosAsync();
    Task<List<Usuario>> ObtenerPorRolAsync(string rol);
    Task AgregarAsync(Usuario usuario);
    Task ActualizarAsync(Usuario usuario);
    Task<bool> ExistePorCorreoAsync(string correo);
}
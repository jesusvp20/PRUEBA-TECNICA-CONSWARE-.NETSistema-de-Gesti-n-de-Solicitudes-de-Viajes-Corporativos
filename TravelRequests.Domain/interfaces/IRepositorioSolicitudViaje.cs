using TravelRequests.Domain.Entities;

namespace TravelRequests.Domain.Interfaces;

// Repositorio solicitudes viaje
public interface IRepositorioSolicitudViaje
{
    Task<SolicitudViaje?> ObtenerPorIdAsync(int id);
    Task<List<SolicitudViaje>> ObtenerPorUsuarioAsync(int usuarioId);
    Task<List<SolicitudViaje>> ObtenerTodosAsync();
    Task AgregarAsync(SolicitudViaje solicitud);
    Task ActualizarAsync(SolicitudViaje solicitud);
}


using TravelRequests.Application.DTOs;

namespace TravelRequests.Application.Servicies;

public interface ISolicitudesViajeService
{
    Task<SolicitudViajeResponseDto> CrearSolicitudAsync(CrearSolicitudViajeDto dto, int usuarioId);
    Task<List<SolicitudViajeResponseDto>> ObtenerSolicitudesPorUsuarioAsync(int usuarioId);
    Task<List<SolicitudViajeResponseDto>> ObtenerTodasSolicitudesAsync();
    Task<SolicitudViajeResponseDto> CambiarEstadoAsync(int solicitudId, CambiarEstadoSolicitudDto dto, string rolUsuario);
}


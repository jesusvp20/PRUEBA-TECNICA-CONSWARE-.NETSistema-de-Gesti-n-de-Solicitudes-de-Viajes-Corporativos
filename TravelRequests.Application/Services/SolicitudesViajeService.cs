using Microsoft.Extensions.Logging;
using TravelRequests.Application.DTOs;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;
using TravelRequests.Domain.Interfaces;

namespace TravelRequests.Application.Servicies;

// Servicio solicitudes viaje
public class SolicitudesViajeService : ISolicitudesViajeService
{
    private readonly IRepositorioSolicitudViaje _repositorio;
    private readonly ILogger<SolicitudesViajeService> _logger;

    public SolicitudesViajeService(
        IRepositorioSolicitudViaje repositorio,
        ILogger<SolicitudesViajeService> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    // Crear solicitud
    public async Task<SolicitudViajeResponseDto> CrearSolicitudAsync(CrearSolicitudViajeDto dto, int usuarioId)
    {
        // Validar origen != destino
        if (dto.CiudadOrigen.Equals(dto.CiudadDestino, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Ciudad origen y destino deben ser diferentes");

        // Validar fecha regreso > fecha ida
        if (dto.FechaRegreso <= dto.FechaIda)
            throw new ArgumentException("Fecha regreso debe ser mayor a fecha ida");

        // Crear nueva solicitud con estado Pendiente
        var solicitud = new SolicitudViaje
        {
            CiudadOrigen = dto.CiudadOrigen,
            CiudadDestino = dto.CiudadDestino,
            FechaIda = dto.FechaIda,
            FechaRegreso = dto.FechaRegreso,
            Justificacion = dto.Justificacion,
            Estado = EstadoSolicitud.Pendiente, // Estado inicial
            FechaCreacion = DateTime.Now,
            UsuarioId = usuarioId
        };

        await _repositorio.AgregarAsync(solicitud);
        _logger.LogInformation("Solicitud creada: {Id}", solicitud.Id);

        return MapToDto(solicitud);
    }

    // Listar solicitudes del usuario
    public async Task<List<SolicitudViajeResponseDto>> ObtenerSolicitudesPorUsuarioAsync(int usuarioId)
    {
        var solicitudes = await _repositorio.ObtenerPorUsuarioAsync(usuarioId);
        return solicitudes.Select(MapToDto).ToList();
    }

    // Listar todas (para Aprobadores)
    public async Task<List<SolicitudViajeResponseDto>> ObtenerTodasSolicitudesAsync()
    {
        var solicitudes = await _repositorio.ObtenerTodosAsync();
        return solicitudes.Select(MapToDto).ToList();
    }

    // Cambiar estado (solo Aprobador)
    public async Task<SolicitudViajeResponseDto> CambiarEstadoAsync(int solicitudId, CambiarEstadoSolicitudDto dto, string rolUsuario)
    {
        // Validar rol Aprobador
        if (rolUsuario != "Aprobador")
            throw new UnauthorizedAccessException("Solo Aprobadores pueden cambiar estado");

        var solicitud = await _repositorio.ObtenerPorIdAsync(solicitudId)
            ?? throw new ArgumentException("Solicitud no encontrada");

        // Validar estado válido (Aprobada/Rechazada)
        if (!Enum.TryParse<EstadoSolicitud>(dto.Estado, true, out var nuevoEstado))
            throw new ArgumentException("Estado inválido (Aprobada/Rechazada)");

        // No permitir volver a Pendiente
        if (nuevoEstado == EstadoSolicitud.Pendiente)
            throw new ArgumentException("No se puede cambiar a Pendiente");

        // Actualizar estado
        solicitud.Estado = nuevoEstado;
        await _repositorio.ActualizarAsync(solicitud);

        _logger.LogInformation("Solicitud {Id} cambiada a {Estado}", solicitudId, nuevoEstado);

        return MapToDto(solicitud);
    }

    private SolicitudViajeResponseDto MapToDto(SolicitudViaje s) => new()
    {
        Id = s.Id,
        CiudadOrigen = s.CiudadOrigen,
        CiudadDestino = s.CiudadDestino,
        FechaIda = s.FechaIda.ToString("yyyy-MM-dd"),
        FechaRegreso = s.FechaRegreso.ToString("yyyy-MM-dd"),
        Justificacion = s.Justificacion,
        Estado = s.Estado.ToString(),
        FechaCreacion = s.FechaCreacion.ToString("yyyy-MM-dd HH:mm"),
        UsuarioId = s.UsuarioId,
        NombreUsuario = s.Usuario?.Nombre ?? ""
    };
}


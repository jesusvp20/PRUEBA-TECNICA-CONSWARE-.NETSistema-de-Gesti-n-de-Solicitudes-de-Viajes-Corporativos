using System.ComponentModel;

namespace TravelRequests.Application.DTOs;

// DTO crear solicitud
public class CrearSolicitudViajeDto
{
    [DefaultValue("Barranquilla")]
    public string CiudadOrigen { get; set; } = string.Empty;
    
    [DefaultValue("Medellin")]
    public string CiudadDestino { get; set; } = string.Empty;
    
    [DefaultValue("2025-12-15")]
    public DateTime FechaIda { get; set; }
    
    [DefaultValue("2025-12-20")]
    public DateTime FechaRegreso { get; set; }
    
    [DefaultValue("Turismo")]
    public string Justificacion { get; set; } = string.Empty;
}

// DTO cambiar estado
public class CambiarEstadoSolicitudDto
{
    public string Estado { get; set; } = string.Empty; // Aprobada, Rechazada
}

// DTO respuesta solicitud
public class SolicitudViajeResponseDto
{
    public int Id { get; set; }
    public string CiudadOrigen { get; set; } = string.Empty;
    public string CiudadDestino { get; set; } = string.Empty;
    public string FechaIda { get; set; } = string.Empty;
    public string FechaRegreso { get; set; } = string.Empty;
    public string Justificacion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string FechaCreacion { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
}


using TravelRequests.Domain.Enums;

namespace TravelRequests.Domain.Entities;

public class SolicitudViaje
{
    public int Id { get; set; }
    public string CiudadOrigen { get; set; } = string.Empty;
    public string CiudadDestino { get; set; } = string.Empty;
    public DateTime FechaIda { get; set; }
    public DateTime FechaRegreso { get; set; }
    public string Justificacion { get; set; } = string.Empty;
    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;
    public DateTime FechaCreacion { get; set; }
    public int UsuarioId { get; set; }
    public virtual Usuario Usuario { get; set; } = null!;
}

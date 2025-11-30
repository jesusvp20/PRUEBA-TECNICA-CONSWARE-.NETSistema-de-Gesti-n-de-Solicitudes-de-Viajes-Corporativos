using TravelRequests.Domain.Enums;

namespace TravelRequests.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Contraseña { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
    public virtual ICollection<SolicitudViaje> SolicitudesViaje { get; set; } = new List<SolicitudViaje>();
}

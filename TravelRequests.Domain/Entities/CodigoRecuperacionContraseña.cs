using TravelRequests.Domain.Entities;

public class CodigoRecuperacionContraseña
{
    public Guid Id { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public DateTime FechaGeneracion { get; set; }
    public DateTime FechaExpiracion { get; set; }
    public bool EstaUsado { get; set; }
    public bool Activo { get; set; } = true;
    public int UsuarioId { get; set; }
    public virtual Usuario Usuario { get; set; } = null!;
}

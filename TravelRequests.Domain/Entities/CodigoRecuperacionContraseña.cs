using System;
using System.Collections.Generic;
using System.Text;
using TravelRequests.Domain.Entities;

public class CodigoRecuperacionContraseña
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public DateTime FechaExpiracion { get; set; }
    public bool EstaUsado { get; set; }

    public Guid UsuarioId { get; set; }
    public virtual Usuario Usuario { get; set; } = null!;
}
using System;
using System.Collections.Generic;
using System.Text;
using TravelRequests.Domain.Enums;
namespace TravelRequests.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;

        public String HasContraseña { get; set; } = string.Empty;

        public RolUsuario Rol { get; set; }
        public DateTime FechaCreacion { get; set; }

        public virtual ICollection<SolicitudViaje> SolictudViaje { get; set; }= new List<SolicitudViaje>();

        public virtual CodigoRecuperacionContraseña? CodigoRecuperacion { get; set; }

    }
}

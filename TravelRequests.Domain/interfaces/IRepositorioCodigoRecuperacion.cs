
namespace TravelRequests.Domain.interfaces;

public interface IRepositorioCodigoRecuperacion
{
    Task<CodigoRecuperacionContraseña?> ObtenerCodigoValidoAsync(string correo, string codigo);
    Task AgregarAsync(CodigoRecuperacionContraseña codigo);
    Task ActualizarAsync(CodigoRecuperacionContraseña codigo);
    Task InvalidarCodigosUsuarioAsync(int usuarioId);
}

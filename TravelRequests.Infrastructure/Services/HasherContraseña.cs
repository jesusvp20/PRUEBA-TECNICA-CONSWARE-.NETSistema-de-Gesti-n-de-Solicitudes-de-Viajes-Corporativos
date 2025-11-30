using TravelRequests.Domain.Interfaces;

namespace TravelRequests.Infrastructure.Services;

public class HasherContraseña : IHasherContraseña
{
    // se utiliza la librería BCrypt para el hasheo de contraseñas
    public string HashContraseña(string contraseña)
    {

        return BCrypt.Net.BCrypt.HashPassword(contraseña);
    }

    /// Verifica si una contraseña coincide con su hash almacenado
    public bool VerificarContraseña(string contraseña, string hashContraseña)
    {
        return BCrypt.Net.BCrypt.Verify(contraseña, hashContraseña);
    }
}
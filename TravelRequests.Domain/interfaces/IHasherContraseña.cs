namespace TravelRequests.Domain.Interfaces;

public interface IHasherContraseña
{
    string HashContraseña(string contraseña);
    bool VerificarContraseña(string contraseña, string hashContraseña);
}
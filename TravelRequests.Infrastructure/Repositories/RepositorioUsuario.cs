        using Microsoft.EntityFrameworkCore;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Interfaces;
using TravelRequests.Infrastructure.Data;

namespace TravelRequests.Infrastructure.Repositorios;

public class RepositorioUsuario : IRepositorioUsuario
{
    private readonly ApplicationDbContext _context;
    public RepositorioUsuario(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObtenerPorIdAsync(int id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == correo);
    }

    public async Task<List<Usuario>> ObtenerTodosAsync()
    {
        return await _context.Usuarios.ToListAsync();
    }

    public async Task<List<Usuario>> ObtenerPorRolAsync(string rol)
    {
        return await _context.Usuarios
            .Where(u => u.Rol.ToString() == rol)
            .ToListAsync();
    }

    public async Task AgregarAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistePorCorreoAsync(string correo)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.Correo == correo);
    }
}
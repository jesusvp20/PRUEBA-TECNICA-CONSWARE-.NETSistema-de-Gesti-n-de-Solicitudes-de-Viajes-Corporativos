using Microsoft.EntityFrameworkCore;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Interfaces;
using TravelRequests.Infrastructure.Data;

namespace TravelRequests.Infrastructure.Repositories;

public class RepositorioSolicitudViaje : IRepositorioSolicitudViaje
{
    private readonly ApplicationDbContext _context;

    public RepositorioSolicitudViaje(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SolicitudViaje?> ObtenerPorIdAsync(int id)
    {
        return await _context.SolicitudesViaje
            .Include(s => s.Usuario)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<SolicitudViaje>> ObtenerPorUsuarioAsync(int usuarioId)
    {
        return await _context.SolicitudesViaje
            .Include(s => s.Usuario)
            .Where(s => s.UsuarioId == usuarioId)
            .OrderByDescending(s => s.FechaCreacion)
            .ToListAsync();
    }

    public async Task<List<SolicitudViaje>> ObtenerTodosAsync()
    {
        return await _context.SolicitudesViaje
            .Include(s => s.Usuario)
            .OrderByDescending(s => s.FechaCreacion)
            .ToListAsync();
    }

    public async Task AgregarAsync(SolicitudViaje solicitud)
    {
        await _context.SolicitudesViaje.AddAsync(solicitud);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(SolicitudViaje solicitud)
    {
        _context.SolicitudesViaje.Update(solicitud);
        await _context.SaveChangesAsync();
    }
}


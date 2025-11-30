using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.interfaces;
using TravelRequests.Infrastructure.Data;

namespace TravelRequests.Infrastructure.Repositories
{
    public class RepositorioCodigoRecuperacion : IRepositorioCodigoRecuperacion
    {
        private readonly ApplicationDbContext _context;

        public RepositorioCodigoRecuperacion(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CodigoRecuperacionContraseña?> ObtenerCodigoValidoAsync(string correo, string codigo)
        {
            return await _context.CodigosRecuperacion
                .FirstOrDefaultAsync(c => c.Correo == correo
                    && c.Codigo == codigo
                    && c.FechaExpiracion > DateTime.Now
                    && c.Activo);
        }

        public async Task AgregarAsync(CodigoRecuperacionContraseña codigo)
        {
            await _context.CodigosRecuperacion.AddAsync(codigo);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(CodigoRecuperacionContraseña codigo)
        {
            _context.CodigosRecuperacion.Update(codigo);
            await _context.SaveChangesAsync();
        }

        public async Task InvalidarCodigosUsuarioAsync(int usuarioId)
        {
            // Eliminar códigos anteriores del usuario
            var codigosAnteriores = await _context.CodigosRecuperacion
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            if (codigosAnteriores.Any())
            {
                _context.CodigosRecuperacion.RemoveRange(codigosAnteriores);
                await _context.SaveChangesAsync();
            }
        }
    }
}
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TravelRequests.Domain.Entities;

namespace TravelRequests.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<SolicitudViaje> SolicitudesViaje { get; set; }
        public DbSet<CodigoRecuperacionContraseña> CodigosRecuperacion { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //configuracion de la entidad Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Correo).IsUnique();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Correo).IsRequired().HasMaxLength(100);
                entity.Property(e=>e.HasContraseña).IsRequired();
                entity.Property(e => e.Rol).IsRequired();

                //relacion uno a muchos entre usuario y solicitud de viaje
                entity.HasMany(u => u.SolictudViaje)
                .WithOne(s => s.Usuario)
                .HasForeignKey(sv => sv.UsuarioId);
            });

            //configuracion de la entidad solicitud de viaje
            modelBuilder.Entity<SolicitudViaje>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CiudadOrigen).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CiudadDestino).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Justificacion).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Estado).IsRequired();

                // Validación: FechaRegreso > FechaIda
                entity.ToTable(tb => tb.HasCheckConstraint("CK_SolicitudViaje_Fechas", "[FechaRegreso] > [FechaIda]"));
            });

            modelBuilder.Entity<CodigoRecuperacionContraseña>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(10);
                entity.Property(e => e.FechaExpiracion).IsRequired();

                // Relación uno-a-uno con Usuario
                entity.HasOne(e => e.Usuario)
                      .WithOne(u => u.CodigoRecuperacion)
                      .HasForeignKey<CodigoRecuperacionContraseña>(e => e.UsuarioId);
            });

        }


    }

}

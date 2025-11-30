
using Microsoft.EntityFrameworkCore;
using TravelRequests.Domain.Entities;

namespace TravelRequests.Infrastructure.Data;

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

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();  
            
            entity.HasIndex(e => e.Correo).IsUnique();
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Correo).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Contraseña).IsRequired();
            entity.Property(e => e.Rol).IsRequired();

            // Relación: Usuario -> Muchas SolicitudesViaje
            entity.HasMany(u => u.SolicitudesViaje)
                  .WithOne(s => s.Usuario)
                  .HasForeignKey(s => s.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

       
        //  Validación fechas
   
        modelBuilder.Entity<SolicitudViaje>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            
            entity.Property(e => e.CiudadOrigen).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CiudadDestino).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Justificacion).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Estado).IsRequired();

            // validacion: FechaRegreso > FechaIda
            entity.ToTable(tb => 
                tb.HasCheckConstraint("CK_SolicitudViaje_Fechas", "[FechaRegreso] > [FechaIda]"));
        });

        // codigo recuperacion 
  
        modelBuilder.Entity<CodigoRecuperacionContraseña>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(10);
            entity.Property(e => e.FechaExpiracion).IsRequired();

            entity.HasOne(e => e.Usuario)
                  .WithMany()
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

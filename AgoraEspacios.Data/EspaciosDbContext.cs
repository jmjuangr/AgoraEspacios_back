using Microsoft.EntityFrameworkCore;
using AgoraEspacios.Models.Entities;

namespace AgoraEspacios.Data
{
    // puente entre mis clases y SQL Server
    // Creo tablas, relaciones y meto reglas
    public class EspaciosDbContext : DbContext
    {

        public EspaciosDbContext(DbContextOptions<EspaciosDbContext> options) : base(options) { }


        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<CategoriaEspacio> CategoriaEspacios { get; set; }
        public DbSet<Espacio> Espacios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Uusuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                // Clave primaria 
                entity.HasKey(u => u.Id);

                // Propiedades básicas 
                entity.Property(u => u.Nombre)
                      .IsRequired()
                      .HasMaxLength(80);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(120);

                // indice unico para evitar emails duplicados
                entity.HasIndex(u => u.Email)
                      .IsUnique();

                entity.Property(u => u.PasswordHash)
                      .IsRequired();
                // Rol como texto
                entity.Property(u => u.Rol)
                      .IsRequired()
                      .HasMaxLength(20);
            });


            //Espacio
            modelBuilder.Entity<CategoriaEspacio>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Nombre)
                      .IsRequired()
                      .HasMaxLength(60);

                // Una única categoria 
                entity.HasIndex(c => c.Nombre)
                      .IsUnique();

                entity.Property(c => c.Descripcion)
                      .HasMaxLength(200);
            });

            //Espacio
            modelBuilder.Entity<Espacio>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                      .IsRequired()
                      .HasMaxLength(100);

                // Capacidad obligatoria
                entity.Property(e => e.Capacidad)
                      .IsRequired();

                entity.Property(e => e.Ubicacion)
                      .HasMaxLength(150);

                entity.Property(e => e.Descripcion)
                      .HasMaxLength(500);

                entity.Property(e => e.ImagenUrl)
                      .HasMaxLength(300);

                // Relaciónes. Un Espacio pertenece a una Categoría (FK CategoriaEspacioId)
                //           Una Categoría tiene n Espacios
                entity.HasOne(e => e.Categoria)
                      .WithMany(c => c.Espacios)
                      .HasForeignKey(e => e.CategoriaEspacioId)
                      //Restringir borrar categoria con espacios
                      .OnDelete(DeleteBehavior.Restrict);
            });

            //REserva

            modelBuilder.Entity<Reserva>(entity =>
            {
                entity.HasKey(r => r.Id);

                // Campos obligatorios
                entity.Property(r => r.FechaInicio).IsRequired();
                entity.Property(r => r.FechaFin).IsRequired();

                // Estado 
                entity.Property(r => r.Estado)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(r => r.Titulo)
                      .HasMaxLength(200);

                // Relaciones. Una Reserva es de UN Usuario y un Usuario puede tener n Reservas
                entity.HasOne(r => r.Usuario)
                      .WithMany(u => u.Reservas)
                      .HasForeignKey(r => r.UsuarioId)
                      // Si se borra un Usuario también se borran sus reservas
                      .OnDelete(DeleteBehavior.Cascade);

                // Relaciones. Una Reserva es para UN Espacio, un Espacio puede tener n Reservas
                entity.HasOne(r => r.Espacio)
                      .WithMany(e => e.Reservas)
                      .HasForeignKey(r => r.EspacioId)
                      // Si el Espacio tiene reservas no se puede borrar
                      .OnDelete(DeleteBehavior.Restrict);

                // Restricción de combrobación (Check constraint) la fecha fin debe ser posterior a la fecha inicio
                entity.ToTable(t =>
     {
         t.HasCheckConstraint("CK_Reserva_Fechas", "[FechaFin] > [FechaInicio]");
     });
            });


        }
    }
}

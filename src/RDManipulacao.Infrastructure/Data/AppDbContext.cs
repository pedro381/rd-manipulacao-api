using Microsoft.EntityFrameworkCore;
using RDManipulacao.Domain.Entities;

namespace RDManipulacao.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Video> Videos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração da entidade Video
            modelBuilder.Entity<Video>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Titulo)
                      .IsRequired()
                      .HasMaxLength(200);
                entity.Property(v => v.Descricao)
                      .HasMaxLength(1000);
                entity.Property(v => v.Autor)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(v => v.Duracao)
                      .HasMaxLength(50);
                entity.Property(v => v.Url)
                      .HasMaxLength(500);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

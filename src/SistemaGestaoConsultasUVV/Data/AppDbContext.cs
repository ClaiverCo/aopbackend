using Microsoft.EntityFrameworkCore;
using SistemaGestaoConsultasUVV.Models;

namespace SistemaGestaoConsultasUVV.Data;

/// <summary>
/// Contexto de dados (EF Core). Registrado no contêiner de DI em <c>Program.cs</c>.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Consulta> Consultas => Set<Consulta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // E-mail único por usuário.
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Relacionamento 1-N Usuario -> Consulta. Ao excluir o usuário,
        // suas consultas também são removidas.
        modelBuilder.Entity<Consulta>()
            .HasOne(c => c.Usuario)
            .WithMany(u => u.Consultas)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

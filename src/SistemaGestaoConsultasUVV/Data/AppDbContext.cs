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
    public DbSet<Medico> Medicos => Set<Medico>();

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

        // Relacionamento 1-N Medico -> Consulta. Restrict evita "múltiplos
        // caminhos de cascata" no SQL Server (o usuário já cascateia).
        modelBuilder.Entity<Consulta>()
            .HasOne(c => c.Medico)
            .WithMany(m => m.Consultas)
            .HasForeignKey(c => c.MedicoId)
            .OnDelete(DeleteBehavior.Restrict);

        SeedMedicos(modelBuilder);
    }

    /// <summary>Corpo clínico fictício, pré-cadastrado por especialidade.</summary>
    private static void SeedMedicos(ModelBuilder modelBuilder)
    {
        const string clinica = "Clínica Médica (Clínico Geral)";
        const string pediatria = "Pediatria";
        const string ginecologia = "Ginecologia e Obstetrícia";
        const string cardiologia = "Cardiologia";
        const string ortopedia = "Ortopedia e Traumatologia";
        const string dermatologia = "Dermatologia";
        const string oftalmologia = "Oftalmologia";

        modelBuilder.Entity<Medico>().HasData(
            new Medico { Id = 1, Nome = "Helena Marques", Especialidade = clinica, Crm = "CRM-ES 10231",
                Resumo = "Cuida da saúde de adultos, trata doenças comuns e faz o direcionamento para especialistas." },
            new Medico { Id = 2, Nome = "Rafael Toledo", Especialidade = clinica, Crm = "CRM-ES 10488",
                Resumo = "Atendimento clínico geral, check-up e acompanhamento de doenças crônicas." },

            new Medico { Id = 3, Nome = "Beatriz Nogueira", Especialidade = pediatria, Crm = "CRM-ES 11002",
                Resumo = "Acompanhamento do crescimento, prevenção e tratamento de doenças em bebês, crianças e adolescentes." },
            new Medico { Id = 4, Nome = "Thiago Ramalho", Especialidade = pediatria, Crm = "CRM-ES 11079",
                Resumo = "Puericultura, vacinação e cuidados pediátricos de rotina." },

            new Medico { Id = 5, Nome = "Carolina Bastos", Especialidade = ginecologia, Crm = "CRM-ES 12140",
                Resumo = "Saúde reprodutiva e íntima da mulher, além do acompanhamento da gravidez e do parto." },
            new Medico { Id = 6, Nome = "Priscila Andrade", Especialidade = ginecologia, Crm = "CRM-ES 12233",
                Resumo = "Consultas ginecológicas de rotina, planejamento familiar e pré-natal." },

            new Medico { Id = 7, Nome = "Anderson Vieira", Especialidade = cardiologia, Crm = "CRM-ES 13051",
                Resumo = "Trata do coração e do sistema circulatório, como pressão alta e exames de rotina preventiva." },
            new Medico { Id = 8, Nome = "Marina Cavalcanti", Especialidade = cardiologia, Crm = "CRM-ES 13188",
                Resumo = "Avaliação de risco cardiovascular, eletrocardiograma e acompanhamento de hipertensão." },

            new Medico { Id = 9, Nome = "Gustavo Peixoto", Especialidade = ortopedia, Crm = "CRM-ES 14075",
                Resumo = "Cuida de problemas nos ossos, músculos, articulações e dores nas costas ou nos membros." },
            new Medico { Id = 10, Nome = "Leonardo Faria", Especialidade = ortopedia, Crm = "CRM-ES 14202",
                Resumo = "Lesões esportivas, fraturas e reabilitação do sistema locomotor." },

            new Medico { Id = 11, Nome = "Luísa Fontes", Especialidade = dermatologia, Crm = "CRM-ES 15019",
                Resumo = "Trata de doenças da pele, cabelos e unhas." },
            new Medico { Id = 12, Nome = "Bruno Salgado", Especialidade = dermatologia, Crm = "CRM-ES 15144",
                Resumo = "Dermatologia clínica, acne, alergias de pele e avaliação de manchas." },

            new Medico { Id = 13, Nome = "Eduardo Lins", Especialidade = oftalmologia, Crm = "CRM-ES 16033",
                Resumo = "Cuida da saúde dos olhos e da visão." },
            new Medico { Id = 14, Nome = "Tatiana Rocha", Especialidade = oftalmologia, Crm = "CRM-ES 16170",
                Resumo = "Exames de vista, adaptação de óculos e acompanhamento de doenças oculares." }
        );
    }
}

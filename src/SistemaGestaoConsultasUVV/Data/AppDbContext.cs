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

        // Um médico não pode ter duas consultas no MESMO horário — o horário
        // fica indisponível assim que alguém o reserva.
        modelBuilder.Entity<Consulta>()
            .HasIndex(c => new { c.MedicoId, c.DataHora })
            .IsUnique();

        SeedMedicos(modelBuilder);
        SeedPacientesEConsultas(modelBuilder);
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

    /// <summary>
    /// Pacientes fictícios e consultas já reservadas, para que existam horários
    /// indisponíveis. A senha de todos é <c>Paciente@123</c>. Dados fixos (gerados
    /// uma vez) para não quebrar as migrations.
    /// </summary>
    private static void SeedPacientesEConsultas(ModelBuilder modelBuilder)
    {
        const string senhaPacienteDemo =
            "AQAAAAIAAYagAAAAEEKGBmDdGTsidMiEV29WxyhM6+h2x6HnF9uE84PAUqyQNISeUzUaLmrkscoD2Nn1Pw==";

        modelBuilder.Entity<Usuario>().HasData(
            new Usuario { Id = 1, Nome = "Ana Beatriz Lima", Email = "ana.lima@exemplo.com", Senha = senhaPacienteDemo, DataCadastro = new DateTime(2026, 8, 1, 9, 0, 0) },
            new Usuario { Id = 2, Nome = "Carlos Henrique Souza", Email = "carlos.souza@exemplo.com", Senha = senhaPacienteDemo, DataCadastro = new DateTime(2026, 8, 2, 9, 0, 0) },
            new Usuario { Id = 3, Nome = "Fernanda Rocha", Email = "fernanda.rocha@exemplo.com", Senha = senhaPacienteDemo, DataCadastro = new DateTime(2026, 8, 3, 9, 0, 0) },
            new Usuario { Id = 4, Nome = "João Pedro Almeida", Email = "joao.almeida@exemplo.com", Senha = senhaPacienteDemo, DataCadastro = new DateTime(2026, 8, 4, 9, 0, 0) },
            new Usuario { Id = 5, Nome = "Mariana Costa", Email = "mariana.costa@exemplo.com", Senha = senhaPacienteDemo, DataCadastro = new DateTime(2026, 8, 5, 9, 0, 0) },
            new Usuario { Id = 6, Nome = "Paulo Ricardo Nunes", Email = "paulo.nunes@exemplo.com", Senha = senhaPacienteDemo, DataCadastro = new DateTime(2026, 8, 6, 9, 0, 0) },
            new Usuario { Id = 7, Nome = "Renata Dias", Email = "renata.dias@exemplo.com", Senha = senhaPacienteDemo, DataCadastro = new DateTime(2026, 8, 7, 9, 0, 0) },
            new Usuario { Id = 8, Nome = "Tiago Moreira", Email = "tiago.moreira@exemplo.com", Senha = senhaPacienteDemo, DataCadastro = new DateTime(2026, 8, 8, 9, 0, 0) }
        );

        modelBuilder.Entity<Consulta>().HasData(
            new Consulta { Id = 1, Especialidade = "Clínica Médica (Clínico Geral)", DataHora = new DateTime(2027, 5, 6, 15, 30, 0), Descricao = "Avaliação de exames", UsuarioId = 7, MedicoId = 1 },
            new Consulta { Id = 2, Especialidade = "Clínica Médica (Clínico Geral)", DataHora = new DateTime(2026, 12, 28, 15, 30, 0), Descricao = "Primeira avaliação", UsuarioId = 8, MedicoId = 1 },
            new Consulta { Id = 3, Especialidade = "Clínica Médica (Clínico Geral)", DataHora = new DateTime(2026, 12, 10, 10, 0, 0), Descricao = "Retorno", UsuarioId = 4, MedicoId = 1 },
            new Consulta { Id = 4, Especialidade = "Clínica Médica (Clínico Geral)", DataHora = new DateTime(2027, 4, 27, 14, 0, 0), Descricao = "Revisão anual", UsuarioId = 4, MedicoId = 2 },
            new Consulta { Id = 5, Especialidade = "Clínica Médica (Clínico Geral)", DataHora = new DateTime(2027, 5, 17, 11, 30, 0), Descricao = "Encaixe", UsuarioId = 7, MedicoId = 2 },
            new Consulta { Id = 6, Especialidade = "Clínica Médica (Clínico Geral)", DataHora = new DateTime(2026, 11, 24, 14, 0, 0), Descricao = "Consulta de rotina", UsuarioId = 5, MedicoId = 2 },
            new Consulta { Id = 7, Especialidade = "Pediatria", DataHora = new DateTime(2026, 10, 20, 9, 0, 0), Descricao = "Revisão anual", UsuarioId = 4, MedicoId = 3 },
            new Consulta { Id = 8, Especialidade = "Pediatria", DataHora = new DateTime(2026, 11, 24, 10, 0, 0), Descricao = "Primeira avaliação", UsuarioId = 4, MedicoId = 3 },
            new Consulta { Id = 9, Especialidade = "Pediatria", DataHora = new DateTime(2027, 1, 18, 14, 30, 0), Descricao = "Primeira avaliação", UsuarioId = 8, MedicoId = 3 },
            new Consulta { Id = 10, Especialidade = "Pediatria", DataHora = new DateTime(2027, 1, 18, 15, 0, 0), Descricao = "Primeira avaliação", UsuarioId = 7, MedicoId = 4 },
            new Consulta { Id = 11, Especialidade = "Pediatria", DataHora = new DateTime(2026, 11, 30, 8, 0, 0), Descricao = "Encaixe", UsuarioId = 7, MedicoId = 4 },
            new Consulta { Id = 12, Especialidade = "Pediatria", DataHora = new DateTime(2026, 10, 6, 17, 0, 0), Descricao = "Consulta preventiva", UsuarioId = 7, MedicoId = 4 },
            new Consulta { Id = 13, Especialidade = "Ginecologia e Obstetrícia", DataHora = new DateTime(2027, 6, 23, 10, 30, 0), Descricao = "Consulta de rotina", UsuarioId = 6, MedicoId = 5 },
            new Consulta { Id = 14, Especialidade = "Ginecologia e Obstetrícia", DataHora = new DateTime(2027, 3, 22, 14, 0, 0), Descricao = "Retorno", UsuarioId = 8, MedicoId = 5 },
            new Consulta { Id = 15, Especialidade = "Ginecologia e Obstetrícia", DataHora = new DateTime(2026, 9, 14, 8, 30, 0), Descricao = "Avaliação de exames", UsuarioId = 7, MedicoId = 5 },
            new Consulta { Id = 16, Especialidade = "Ginecologia e Obstetrícia", DataHora = new DateTime(2026, 12, 14, 10, 30, 0), Descricao = "Acompanhamento", UsuarioId = 2, MedicoId = 6 },
            new Consulta { Id = 17, Especialidade = "Ginecologia e Obstetrícia", DataHora = new DateTime(2027, 4, 7, 17, 0, 0), Descricao = "Consulta de rotina", UsuarioId = 2, MedicoId = 6 },
            new Consulta { Id = 18, Especialidade = "Ginecologia e Obstetrícia", DataHora = new DateTime(2027, 1, 27, 15, 30, 0), Descricao = "Retorno", UsuarioId = 5, MedicoId = 6 },
            new Consulta { Id = 19, Especialidade = "Cardiologia", DataHora = new DateTime(2027, 2, 10, 10, 0, 0), Descricao = "Retorno", UsuarioId = 7, MedicoId = 7 },
            new Consulta { Id = 20, Especialidade = "Cardiologia", DataHora = new DateTime(2027, 5, 19, 16, 0, 0), Descricao = "Consulta de rotina", UsuarioId = 4, MedicoId = 7 },
            new Consulta { Id = 21, Especialidade = "Cardiologia", DataHora = new DateTime(2027, 5, 26, 17, 0, 0), Descricao = "Acompanhamento", UsuarioId = 1, MedicoId = 7 },
            new Consulta { Id = 22, Especialidade = "Cardiologia", DataHora = new DateTime(2027, 3, 24, 8, 30, 0), Descricao = "Consulta preventiva", UsuarioId = 8, MedicoId = 8 },
            new Consulta { Id = 23, Especialidade = "Cardiologia", DataHora = new DateTime(2027, 2, 1, 15, 30, 0), Descricao = "Primeira avaliação", UsuarioId = 7, MedicoId = 8 },
            new Consulta { Id = 24, Especialidade = "Cardiologia", DataHora = new DateTime(2027, 5, 11, 17, 0, 0), Descricao = "Retorno", UsuarioId = 3, MedicoId = 8 },
            new Consulta { Id = 25, Especialidade = "Ortopedia e Traumatologia", DataHora = new DateTime(2027, 4, 19, 8, 0, 0), Descricao = "Retorno", UsuarioId = 5, MedicoId = 9 },
            new Consulta { Id = 26, Especialidade = "Ortopedia e Traumatologia", DataHora = new DateTime(2027, 1, 1, 11, 30, 0), Descricao = "Revisão anual", UsuarioId = 7, MedicoId = 9 },
            new Consulta { Id = 27, Especialidade = "Ortopedia e Traumatologia", DataHora = new DateTime(2027, 6, 11, 8, 0, 0), Descricao = "Encaixe", UsuarioId = 7, MedicoId = 9 },
            new Consulta { Id = 28, Especialidade = "Ortopedia e Traumatologia", DataHora = new DateTime(2027, 5, 14, 16, 30, 0), Descricao = "Avaliação de exames", UsuarioId = 8, MedicoId = 10 },
            new Consulta { Id = 29, Especialidade = "Ortopedia e Traumatologia", DataHora = new DateTime(2027, 3, 12, 8, 30, 0), Descricao = "Consulta de rotina", UsuarioId = 2, MedicoId = 10 },
            new Consulta { Id = 30, Especialidade = "Ortopedia e Traumatologia", DataHora = new DateTime(2026, 9, 22, 15, 30, 0), Descricao = "Retorno", UsuarioId = 1, MedicoId = 10 },
            new Consulta { Id = 31, Especialidade = "Dermatologia", DataHora = new DateTime(2027, 4, 16, 11, 0, 0), Descricao = "Retorno", UsuarioId = 1, MedicoId = 11 },
            new Consulta { Id = 32, Especialidade = "Dermatologia", DataHora = new DateTime(2027, 3, 25, 17, 0, 0), Descricao = "Primeira avaliação", UsuarioId = 6, MedicoId = 11 },
            new Consulta { Id = 33, Especialidade = "Dermatologia", DataHora = new DateTime(2026, 12, 31, 16, 30, 0), Descricao = "Encaixe", UsuarioId = 3, MedicoId = 11 },
            new Consulta { Id = 34, Especialidade = "Dermatologia", DataHora = new DateTime(2027, 4, 8, 16, 0, 0), Descricao = "Consulta de rotina", UsuarioId = 7, MedicoId = 12 },
            new Consulta { Id = 35, Especialidade = "Dermatologia", DataHora = new DateTime(2026, 12, 11, 15, 0, 0), Descricao = "Primeira avaliação", UsuarioId = 8, MedicoId = 12 },
            new Consulta { Id = 36, Especialidade = "Dermatologia", DataHora = new DateTime(2027, 4, 28, 11, 0, 0), Descricao = "Consulta preventiva", UsuarioId = 3, MedicoId = 12 },
            new Consulta { Id = 37, Especialidade = "Oftalmologia", DataHora = new DateTime(2027, 3, 3, 16, 30, 0), Descricao = "Avaliação de exames", UsuarioId = 6, MedicoId = 13 },
            new Consulta { Id = 38, Especialidade = "Oftalmologia", DataHora = new DateTime(2026, 12, 3, 11, 0, 0), Descricao = "Acompanhamento", UsuarioId = 8, MedicoId = 13 },
            new Consulta { Id = 39, Especialidade = "Oftalmologia", DataHora = new DateTime(2027, 5, 3, 9, 30, 0), Descricao = "Retorno", UsuarioId = 3, MedicoId = 13 },
            new Consulta { Id = 40, Especialidade = "Oftalmologia", DataHora = new DateTime(2026, 12, 18, 9, 0, 0), Descricao = "Encaixe", UsuarioId = 1, MedicoId = 14 },
            new Consulta { Id = 41, Especialidade = "Oftalmologia", DataHora = new DateTime(2027, 6, 8, 8, 0, 0), Descricao = "Encaixe", UsuarioId = 4, MedicoId = 14 },
            new Consulta { Id = 42, Especialidade = "Oftalmologia", DataHora = new DateTime(2027, 5, 25, 14, 0, 0), Descricao = "Acompanhamento", UsuarioId = 3, MedicoId = 14 }
        );
    }
}

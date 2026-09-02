using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaGestaoConsultasUVV.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedicoId",
                table: "Consultas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Medicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Especialidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Crm = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Resumo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Medicos",
                columns: new[] { "Id", "Crm", "Especialidade", "Nome", "Resumo" },
                values: new object[,]
                {
                    { 1, "CRM-ES 10231", "Clínica Médica (Clínico Geral)", "Helena Marques", "Cuida da saúde de adultos, trata doenças comuns e faz o direcionamento para especialistas." },
                    { 2, "CRM-ES 10488", "Clínica Médica (Clínico Geral)", "Rafael Toledo", "Atendimento clínico geral, check-up e acompanhamento de doenças crônicas." },
                    { 3, "CRM-ES 11002", "Pediatria", "Beatriz Nogueira", "Acompanhamento do crescimento, prevenção e tratamento de doenças em bebês, crianças e adolescentes." },
                    { 4, "CRM-ES 11079", "Pediatria", "Thiago Ramalho", "Puericultura, vacinação e cuidados pediátricos de rotina." },
                    { 5, "CRM-ES 12140", "Ginecologia e Obstetrícia", "Carolina Bastos", "Saúde reprodutiva e íntima da mulher, além do acompanhamento da gravidez e do parto." },
                    { 6, "CRM-ES 12233", "Ginecologia e Obstetrícia", "Priscila Andrade", "Consultas ginecológicas de rotina, planejamento familiar e pré-natal." },
                    { 7, "CRM-ES 13051", "Cardiologia", "Anderson Vieira", "Trata do coração e do sistema circulatório, como pressão alta e exames de rotina preventiva." },
                    { 8, "CRM-ES 13188", "Cardiologia", "Marina Cavalcanti", "Avaliação de risco cardiovascular, eletrocardiograma e acompanhamento de hipertensão." },
                    { 9, "CRM-ES 14075", "Ortopedia e Traumatologia", "Gustavo Peixoto", "Cuida de problemas nos ossos, músculos, articulações e dores nas costas ou nos membros." },
                    { 10, "CRM-ES 14202", "Ortopedia e Traumatologia", "Leonardo Faria", "Lesões esportivas, fraturas e reabilitação do sistema locomotor." },
                    { 11, "CRM-ES 15019", "Dermatologia", "Luísa Fontes", "Trata de doenças da pele, cabelos e unhas." },
                    { 12, "CRM-ES 15144", "Dermatologia", "Bruno Salgado", "Dermatologia clínica, acne, alergias de pele e avaliação de manchas." },
                    { 13, "CRM-ES 16033", "Oftalmologia", "Eduardo Lins", "Cuida da saúde dos olhos e da visão." },
                    { 14, "CRM-ES 16170", "Oftalmologia", "Tatiana Rocha", "Exames de vista, adaptação de óculos e acompanhamento de doenças oculares." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_MedicoId",
                table: "Consultas",
                column: "MedicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultas_Medicos_MedicoId",
                table: "Consultas",
                column: "MedicoId",
                principalTable: "Medicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultas_Medicos_MedicoId",
                table: "Consultas");

            migrationBuilder.DropTable(
                name: "Medicos");

            migrationBuilder.DropIndex(
                name: "IX_Consultas_MedicoId",
                table: "Consultas");

            migrationBuilder.DropColumn(
                name: "MedicoId",
                table: "Consultas");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaGestaoConsultasUVV.Migrations
{
    /// <inheritdoc />
    public partial class HorariosReservadosESeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Consultas_MedicoId",
                table: "Consultas");

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "DataCadastro", "Email", "Nome", "Senha" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 8, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "ana.lima@exemplo.com", "Ana Beatriz Lima", "AQAAAAIAAYagAAAAEEKGBmDdGTsidMiEV29WxyhM6+h2x6HnF9uE84PAUqyQNISeUzUaLmrkscoD2Nn1Pw==" },
                    { 2, new DateTime(2026, 8, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), "carlos.souza@exemplo.com", "Carlos Henrique Souza", "AQAAAAIAAYagAAAAEEKGBmDdGTsidMiEV29WxyhM6+h2x6HnF9uE84PAUqyQNISeUzUaLmrkscoD2Nn1Pw==" },
                    { 3, new DateTime(2026, 8, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), "fernanda.rocha@exemplo.com", "Fernanda Rocha", "AQAAAAIAAYagAAAAEEKGBmDdGTsidMiEV29WxyhM6+h2x6HnF9uE84PAUqyQNISeUzUaLmrkscoD2Nn1Pw==" },
                    { 4, new DateTime(2026, 8, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), "joao.almeida@exemplo.com", "João Pedro Almeida", "AQAAAAIAAYagAAAAEEKGBmDdGTsidMiEV29WxyhM6+h2x6HnF9uE84PAUqyQNISeUzUaLmrkscoD2Nn1Pw==" },
                    { 5, new DateTime(2026, 8, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), "mariana.costa@exemplo.com", "Mariana Costa", "AQAAAAIAAYagAAAAEEKGBmDdGTsidMiEV29WxyhM6+h2x6HnF9uE84PAUqyQNISeUzUaLmrkscoD2Nn1Pw==" },
                    { 6, new DateTime(2026, 8, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), "paulo.nunes@exemplo.com", "Paulo Ricardo Nunes", "AQAAAAIAAYagAAAAEEKGBmDdGTsidMiEV29WxyhM6+h2x6HnF9uE84PAUqyQNISeUzUaLmrkscoD2Nn1Pw==" },
                    { 7, new DateTime(2026, 8, 7, 9, 0, 0, 0, DateTimeKind.Unspecified), "renata.dias@exemplo.com", "Renata Dias", "AQAAAAIAAYagAAAAEEKGBmDdGTsidMiEV29WxyhM6+h2x6HnF9uE84PAUqyQNISeUzUaLmrkscoD2Nn1Pw==" },
                    { 8, new DateTime(2026, 8, 8, 9, 0, 0, 0, DateTimeKind.Unspecified), "tiago.moreira@exemplo.com", "Tiago Moreira", "AQAAAAIAAYagAAAAEEKGBmDdGTsidMiEV29WxyhM6+h2x6HnF9uE84PAUqyQNISeUzUaLmrkscoD2Nn1Pw==" }
                });

            migrationBuilder.InsertData(
                table: "Consultas",
                columns: new[] { "Id", "DataHora", "Descricao", "Especialidade", "MedicoId", "UsuarioId" },
                values: new object[,]
                {
                    { 1, new DateTime(2027, 5, 6, 15, 30, 0, 0, DateTimeKind.Unspecified), "Avaliação de exames", "Clínica Médica (Clínico Geral)", 1, 7 },
                    { 2, new DateTime(2026, 12, 28, 15, 30, 0, 0, DateTimeKind.Unspecified), "Primeira avaliação", "Clínica Médica (Clínico Geral)", 1, 8 },
                    { 3, new DateTime(2026, 12, 10, 10, 0, 0, 0, DateTimeKind.Unspecified), "Retorno", "Clínica Médica (Clínico Geral)", 1, 4 },
                    { 4, new DateTime(2027, 4, 27, 14, 0, 0, 0, DateTimeKind.Unspecified), "Revisão anual", "Clínica Médica (Clínico Geral)", 2, 4 },
                    { 5, new DateTime(2027, 5, 17, 11, 30, 0, 0, DateTimeKind.Unspecified), "Encaixe", "Clínica Médica (Clínico Geral)", 2, 7 },
                    { 6, new DateTime(2026, 11, 24, 14, 0, 0, 0, DateTimeKind.Unspecified), "Consulta de rotina", "Clínica Médica (Clínico Geral)", 2, 5 },
                    { 7, new DateTime(2026, 10, 20, 9, 0, 0, 0, DateTimeKind.Unspecified), "Revisão anual", "Pediatria", 3, 4 },
                    { 8, new DateTime(2026, 11, 24, 10, 0, 0, 0, DateTimeKind.Unspecified), "Primeira avaliação", "Pediatria", 3, 4 },
                    { 9, new DateTime(2027, 1, 18, 14, 30, 0, 0, DateTimeKind.Unspecified), "Primeira avaliação", "Pediatria", 3, 8 },
                    { 10, new DateTime(2027, 1, 18, 15, 0, 0, 0, DateTimeKind.Unspecified), "Primeira avaliação", "Pediatria", 4, 7 },
                    { 11, new DateTime(2026, 11, 30, 8, 0, 0, 0, DateTimeKind.Unspecified), "Encaixe", "Pediatria", 4, 7 },
                    { 12, new DateTime(2026, 10, 6, 17, 0, 0, 0, DateTimeKind.Unspecified), "Consulta preventiva", "Pediatria", 4, 7 },
                    { 13, new DateTime(2027, 6, 23, 10, 30, 0, 0, DateTimeKind.Unspecified), "Consulta de rotina", "Ginecologia e Obstetrícia", 5, 6 },
                    { 14, new DateTime(2027, 3, 22, 14, 0, 0, 0, DateTimeKind.Unspecified), "Retorno", "Ginecologia e Obstetrícia", 5, 8 },
                    { 15, new DateTime(2026, 9, 14, 8, 30, 0, 0, DateTimeKind.Unspecified), "Avaliação de exames", "Ginecologia e Obstetrícia", 5, 7 },
                    { 16, new DateTime(2026, 12, 14, 10, 30, 0, 0, DateTimeKind.Unspecified), "Acompanhamento", "Ginecologia e Obstetrícia", 6, 2 },
                    { 17, new DateTime(2027, 4, 7, 17, 0, 0, 0, DateTimeKind.Unspecified), "Consulta de rotina", "Ginecologia e Obstetrícia", 6, 2 },
                    { 18, new DateTime(2027, 1, 27, 15, 30, 0, 0, DateTimeKind.Unspecified), "Retorno", "Ginecologia e Obstetrícia", 6, 5 },
                    { 19, new DateTime(2027, 2, 10, 10, 0, 0, 0, DateTimeKind.Unspecified), "Retorno", "Cardiologia", 7, 7 },
                    { 20, new DateTime(2027, 5, 19, 16, 0, 0, 0, DateTimeKind.Unspecified), "Consulta de rotina", "Cardiologia", 7, 4 },
                    { 21, new DateTime(2027, 5, 26, 17, 0, 0, 0, DateTimeKind.Unspecified), "Acompanhamento", "Cardiologia", 7, 1 },
                    { 22, new DateTime(2027, 3, 24, 8, 30, 0, 0, DateTimeKind.Unspecified), "Consulta preventiva", "Cardiologia", 8, 8 },
                    { 23, new DateTime(2027, 2, 1, 15, 30, 0, 0, DateTimeKind.Unspecified), "Primeira avaliação", "Cardiologia", 8, 7 },
                    { 24, new DateTime(2027, 5, 11, 17, 0, 0, 0, DateTimeKind.Unspecified), "Retorno", "Cardiologia", 8, 3 },
                    { 25, new DateTime(2027, 4, 19, 8, 0, 0, 0, DateTimeKind.Unspecified), "Retorno", "Ortopedia e Traumatologia", 9, 5 },
                    { 26, new DateTime(2027, 1, 1, 11, 30, 0, 0, DateTimeKind.Unspecified), "Revisão anual", "Ortopedia e Traumatologia", 9, 7 },
                    { 27, new DateTime(2027, 6, 11, 8, 0, 0, 0, DateTimeKind.Unspecified), "Encaixe", "Ortopedia e Traumatologia", 9, 7 },
                    { 28, new DateTime(2027, 5, 14, 16, 30, 0, 0, DateTimeKind.Unspecified), "Avaliação de exames", "Ortopedia e Traumatologia", 10, 8 },
                    { 29, new DateTime(2027, 3, 12, 8, 30, 0, 0, DateTimeKind.Unspecified), "Consulta de rotina", "Ortopedia e Traumatologia", 10, 2 },
                    { 30, new DateTime(2026, 9, 22, 15, 30, 0, 0, DateTimeKind.Unspecified), "Retorno", "Ortopedia e Traumatologia", 10, 1 },
                    { 31, new DateTime(2027, 4, 16, 11, 0, 0, 0, DateTimeKind.Unspecified), "Retorno", "Dermatologia", 11, 1 },
                    { 32, new DateTime(2027, 3, 25, 17, 0, 0, 0, DateTimeKind.Unspecified), "Primeira avaliação", "Dermatologia", 11, 6 },
                    { 33, new DateTime(2026, 12, 31, 16, 30, 0, 0, DateTimeKind.Unspecified), "Encaixe", "Dermatologia", 11, 3 },
                    { 34, new DateTime(2027, 4, 8, 16, 0, 0, 0, DateTimeKind.Unspecified), "Consulta de rotina", "Dermatologia", 12, 7 },
                    { 35, new DateTime(2026, 12, 11, 15, 0, 0, 0, DateTimeKind.Unspecified), "Primeira avaliação", "Dermatologia", 12, 8 },
                    { 36, new DateTime(2027, 4, 28, 11, 0, 0, 0, DateTimeKind.Unspecified), "Consulta preventiva", "Dermatologia", 12, 3 },
                    { 37, new DateTime(2027, 3, 3, 16, 30, 0, 0, DateTimeKind.Unspecified), "Avaliação de exames", "Oftalmologia", 13, 6 },
                    { 38, new DateTime(2026, 12, 3, 11, 0, 0, 0, DateTimeKind.Unspecified), "Acompanhamento", "Oftalmologia", 13, 8 },
                    { 39, new DateTime(2027, 5, 3, 9, 30, 0, 0, DateTimeKind.Unspecified), "Retorno", "Oftalmologia", 13, 3 },
                    { 40, new DateTime(2026, 12, 18, 9, 0, 0, 0, DateTimeKind.Unspecified), "Encaixe", "Oftalmologia", 14, 1 },
                    { 41, new DateTime(2027, 6, 8, 8, 0, 0, 0, DateTimeKind.Unspecified), "Encaixe", "Oftalmologia", 14, 4 },
                    { 42, new DateTime(2027, 5, 25, 14, 0, 0, 0, DateTimeKind.Unspecified), "Acompanhamento", "Oftalmologia", 14, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_MedicoId_DataHora",
                table: "Consultas",
                columns: new[] { "MedicoId", "DataHora" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Consultas_MedicoId_DataHora",
                table: "Consultas");

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Consultas",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_MedicoId",
                table: "Consultas",
                column: "MedicoId");
        }
    }
}

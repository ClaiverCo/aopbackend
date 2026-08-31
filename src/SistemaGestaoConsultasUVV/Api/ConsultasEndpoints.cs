using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SistemaGestaoConsultasUVV.Data;
using SistemaGestaoConsultasUVV.Models;

namespace SistemaGestaoConsultasUVV.Api;

/// <summary>DTO de entrada/saída da API — evita expor a entidade e o ciclo de navegação.</summary>
public record ConsultaDto(int Id, string Especialidade, DateTime DataHora, string Descricao);

/// <summary>Dados aceitos na criação/edição via API.</summary>
public record ConsultaInput(string Especialidade, DateTime DataHora, string Descricao);

/// <summary>
/// Endpoints REST (Minimal API) para Consultas. O grupo é registrado em
/// <c>Program.cs</c> com <c>.RequireAuthorization()</c>, portanto todas as rotas
/// exigem usuário autenticado (401 sem cookie de sessão).
/// </summary>
public static class ConsultasEndpoints
{
    public static RouteGroupBuilder MapConsultasApi(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (ClaimsPrincipal user, AppDbContext db) =>
        {
            var uid = GetUserId(user);
            var consultas = await db.Consultas
                .Where(c => c.UsuarioId == uid)
                .OrderBy(c => c.DataHora)
                .Select(c => new ConsultaDto(c.Id, c.Especialidade, c.DataHora, c.Descricao))
                .ToListAsync();
            return Results.Ok(consultas);
        });

        group.MapGet("/{id:int}", async (int id, ClaimsPrincipal user, AppDbContext db) =>
        {
            var uid = GetUserId(user);
            var c = await db.Consultas.FirstOrDefaultAsync(x => x.Id == id && x.UsuarioId == uid);
            return c is null
                ? Results.NotFound()
                : Results.Ok(new ConsultaDto(c.Id, c.Especialidade, c.DataHora, c.Descricao));
        });

        group.MapPost("/", async (ConsultaInput input, ClaimsPrincipal user, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(input.Especialidade) || string.IsNullOrWhiteSpace(input.Descricao))
                return Results.BadRequest("Especialidade e Descrição são obrigatórias.");

            var consulta = new Consulta
            {
                Especialidade = input.Especialidade,
                DataHora = input.DataHora,
                Descricao = input.Descricao,
                UsuarioId = GetUserId(user)
            };
            db.Consultas.Add(consulta);
            await db.SaveChangesAsync();
            return Results.Created($"/api/consultas/{consulta.Id}",
                new ConsultaDto(consulta.Id, consulta.Especialidade, consulta.DataHora, consulta.Descricao));
        });

        group.MapPut("/{id:int}", async (int id, ConsultaInput input, ClaimsPrincipal user, AppDbContext db) =>
        {
            var uid = GetUserId(user);
            var consulta = await db.Consultas.FirstOrDefaultAsync(x => x.Id == id && x.UsuarioId == uid);
            if (consulta is null) return Results.NotFound();

            consulta.Especialidade = input.Especialidade;
            consulta.DataHora = input.DataHora;
            consulta.Descricao = input.Descricao;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("/{id:int}", async (int id, ClaimsPrincipal user, AppDbContext db) =>
        {
            var uid = GetUserId(user);
            var consulta = await db.Consultas.FirstOrDefaultAsync(x => x.Id == id && x.UsuarioId == uid);
            if (consulta is null) return Results.NotFound();

            db.Consultas.Remove(consulta);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return group;
    }

    private static int GetUserId(ClaimsPrincipal user) =>
        int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

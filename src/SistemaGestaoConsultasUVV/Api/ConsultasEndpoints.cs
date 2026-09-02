using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SistemaGestaoConsultasUVV.Data;
using SistemaGestaoConsultasUVV.Models;

namespace SistemaGestaoConsultasUVV.Api;

/// <summary>DTO de saída — evita expor a entidade e o ciclo de navegação.</summary>
public record ConsultaDto(int Id, int MedicoId, string Medico, string Especialidade, DateTime DataHora, string Descricao);

/// <summary>Dados aceitos na criação/edição via API. A especialidade vem do médico.</summary>
public record ConsultaInput(int MedicoId, DateTime DataHora, string Descricao);

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
                .Include(c => c.Medico)
                .Where(c => c.UsuarioId == uid)
                .OrderBy(c => c.DataHora)
                .ToListAsync();
            return Results.Ok(consultas.Select(ToDto));
        });

        group.MapGet("/{id:int}", async (int id, ClaimsPrincipal user, AppDbContext db) =>
        {
            var uid = GetUserId(user);
            var c = await db.Consultas
                .Include(x => x.Medico)
                .FirstOrDefaultAsync(x => x.Id == id && x.UsuarioId == uid);
            return c is null ? Results.NotFound() : Results.Ok(ToDto(c));
        });

        group.MapPost("/", async (ConsultaInput input, ClaimsPrincipal user, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(input.Descricao))
                return Results.BadRequest("Descrição é obrigatória.");

            var medico = await db.Medicos.FindAsync(input.MedicoId);
            if (medico is null) return Results.BadRequest("Médico inválido.");

            var quando = Consulta.AlinharAoSlot(input.DataHora);
            if (quando <= DateTime.Now)
                return Results.BadRequest("A data/hora deve ser futura.");

            if (await HorarioOcupadoAsync(db, input.MedicoId, quando, null))
                return Results.Conflict("Horário indisponível: já reservado para este médico.");

            var consulta = new Consulta
            {
                MedicoId = medico.Id,
                Especialidade = medico.Especialidade,
                DataHora = quando,
                Descricao = input.Descricao,
                UsuarioId = GetUserId(user)
            };
            db.Consultas.Add(consulta);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Results.Conflict("Horário indisponível: acabou de ser reservado.");
            }

            await db.Entry(consulta).Reference(c => c.Medico).LoadAsync();
            return Results.Created($"/api/consultas/{consulta.Id}", ToDto(consulta));
        });

        group.MapPut("/{id:int}", async (int id, ConsultaInput input, ClaimsPrincipal user, AppDbContext db) =>
        {
            var uid = GetUserId(user);
            var consulta = await db.Consultas.FirstOrDefaultAsync(x => x.Id == id && x.UsuarioId == uid);
            if (consulta is null) return Results.NotFound();

            var medico = await db.Medicos.FindAsync(input.MedicoId);
            if (medico is null) return Results.BadRequest("Médico inválido.");

            var quando = Consulta.AlinharAoSlot(input.DataHora);
            if (quando <= DateTime.Now)
                return Results.BadRequest("A data/hora deve ser futura.");

            if (await HorarioOcupadoAsync(db, input.MedicoId, quando, id))
                return Results.Conflict("Horário indisponível: já reservado para este médico.");

            consulta.MedicoId = medico.Id;
            consulta.Especialidade = medico.Especialidade;
            consulta.DataHora = quando;
            consulta.Descricao = input.Descricao;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Results.Conflict("Horário indisponível: acabou de ser reservado.");
            }
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

    private static Task<bool> HorarioOcupadoAsync(AppDbContext db, int medicoId, DateTime quando, int? ignorarId) =>
        db.Consultas.AnyAsync(c =>
            c.MedicoId == medicoId &&
            c.DataHora == quando &&
            (ignorarId == null || c.Id != ignorarId));

    private static ConsultaDto ToDto(Consulta c) =>
        new(c.Id, c.MedicoId, c.Medico == null ? "" : c.Medico.Nome, c.Especialidade, c.DataHora, c.Descricao);

    private static int GetUserId(ClaimsPrincipal user) =>
        int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

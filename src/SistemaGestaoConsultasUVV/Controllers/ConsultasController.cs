using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaGestaoConsultasUVV.Data;
using SistemaGestaoConsultasUVV.Models;
using SistemaGestaoConsultasUVV.Services;

namespace SistemaGestaoConsultasUVV.Controllers;

/// <summary>
/// CRUD de consultas do usuário autenticado. O atributo [Authorize] na classe
/// protege TODAS as rotas — visitantes anônimos são redirecionados para o login.
/// </summary>
[Authorize]
public class ConsultasController : Controller
{
    private readonly AppDbContext _db;

    public ConsultasController(AppDbContext db) => _db = db;

    private int UsuarioAtualId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        var consultas = await _db.Consultas
            .Include(c => c.Medico)
            .Where(c => c.UsuarioId == UsuarioAtualId)
            .OrderBy(c => c.DataHora)
            .ToListAsync();
        return View(consultas);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var consulta = await BuscarDoUsuarioAsync(id.Value);
        return consulta is null ? NotFound() : View(consulta);
    }

    public async Task<IActionResult> Create()
    {
        await PrepararFormularioAsync();
        return View(new Consulta());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MedicoId,DataHora,Descricao")] Consulta consulta)
    {
        await AplicarMedicoAsync(consulta);
        await ValidarHorarioAsync(consulta, ignorarConsultaId: null);

        if (!ModelState.IsValid)
        {
            await PrepararFormularioAsync(consulta.MedicoId);
            return View(consulta);
        }

        consulta.UsuarioId = UsuarioAtualId; // dono vem sempre das claims, nunca do form
        _db.Consultas.Add(consulta);

        if (!await SalvarComTratamentoDeConflitoAsync(consulta))
            return View(consulta);

        TempData["Sucesso"] = "Consulta agendada com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var consulta = await BuscarDoUsuarioAsync(id.Value);
        if (consulta is null) return NotFound();

        await PrepararFormularioAsync(consulta.MedicoId);
        return View(consulta);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,MedicoId,DataHora,Descricao")] Consulta consulta)
    {
        if (id != consulta.Id) return NotFound();

        var original = await BuscarDoUsuarioAsync(id);
        if (original is null) return NotFound();

        await AplicarMedicoAsync(consulta);
        await ValidarHorarioAsync(consulta, ignorarConsultaId: id);

        if (!ModelState.IsValid)
        {
            await PrepararFormularioAsync(consulta.MedicoId);
            return View(consulta);
        }

        original.MedicoId = consulta.MedicoId;
        original.Especialidade = consulta.Especialidade;
        original.DataHora = consulta.DataHora;
        original.Descricao = consulta.Descricao;

        if (!await SalvarComTratamentoDeConflitoAsync(consulta))
            return View(consulta);

        TempData["Sucesso"] = "Consulta atualizada.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var consulta = await BuscarDoUsuarioAsync(id.Value);
        return consulta is null ? NotFound() : View(consulta);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var consulta = await BuscarDoUsuarioAsync(id);
        if (consulta is not null)
        {
            _db.Consultas.Remove(consulta);
            await _db.SaveChangesAsync();
            TempData["Sucesso"] = "Consulta excluída.";
        }
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// GET: /Consultas/Disponibilidade?medicoId=7&amp;data=2026-09-15
    /// Alimenta o calendário: diz se o dia é útil e o status (livre/ocupado) de
    /// cada horário comercial (08:00–18:00, de 30 em 30 min).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Disponibilidade(int medicoId, DateOnly data)
    {
        bool fimDeSemana = data.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        var feriado = Agenda.Feriado(data);
        bool passado = data < DateOnly.FromDateTime(DateTime.Today);

        if (passado || fimDeSemana || feriado is not null)
        {
            string motivo = passado ? "Data passada"
                          : fimDeSemana ? "Fim de semana"
                          : $"Feriado: {feriado}";
            return Json(new { diaUtil = false, motivo, slots = Array.Empty<object>() });
        }

        var inicio = data.ToDateTime(TimeOnly.MinValue);
        var fim = inicio.AddDays(1);
        var ocupados = (await _db.Consultas
            .Where(c => c.MedicoId == medicoId && c.DataHora >= inicio && c.DataHora < fim)
            .Select(c => c.DataHora)
            .ToListAsync()).ToHashSet();

        var agora = DateTime.Now;
        var slots = Agenda.Slots().Select(t =>
        {
            var dt = data.ToDateTime(t);
            return new
            {
                hora = t.ToString("HH:mm"),
                iso = dt.ToString("yyyy-MM-ddTHH:mm"),
                disponivel = dt > agora && !ocupados.Contains(dt)
            };
        });

        return Json(new { diaUtil = true, motivo = (string?)null, slots });
    }

    private Task<Consulta?> BuscarDoUsuarioAsync(int id) =>
        _db.Consultas
            .Include(c => c.Medico)
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == UsuarioAtualId);

    /// <summary>
    /// Resolve o médico escolhido e copia a especialidade dele para a consulta.
    /// A especialidade não é digitada — vem sempre do cadastro do médico.
    /// </summary>
    private async Task AplicarMedicoAsync(Consulta consulta)
    {
        var medico = await _db.Medicos.FindAsync(consulta.MedicoId);
        if (medico is null)
        {
            ModelState.AddModelError(nameof(consulta.MedicoId), "Selecione um médico válido.");
            return;
        }

        consulta.Especialidade = medico.Especialidade;
    }

    /// <summary>
    /// Repete no servidor as mesmas regras do calendário: horário escolhido,
    /// futuro, em dia útil, dentro do expediente (08:00–18:00) e livre para o médico.
    /// </summary>
    private async Task ValidarHorarioAsync(Consulta consulta, int? ignorarConsultaId)
    {
        consulta.DataHora = Agenda.AlinharAoSlot(consulta.DataHora);
        ModelState.Remove(nameof(consulta.DataHora));

        if (consulta.DataHora == default)
        {
            ModelState.AddModelError(nameof(consulta.DataHora), "Selecione um horário no calendário.");
            return;
        }

        var motivo = Agenda.MotivoHorarioInvalido(consulta.DataHora);
        if (motivo is not null)
        {
            ModelState.AddModelError(nameof(consulta.DataHora), motivo);
            return;
        }

        if (consulta.MedicoId <= 0) return; // erro de médico já reportado

        bool ocupado = await _db.Consultas.AnyAsync(c =>
            c.MedicoId == consulta.MedicoId &&
            c.DataHora == consulta.DataHora &&
            (ignorarConsultaId == null || c.Id != ignorarConsultaId));

        if (ocupado)
            ModelState.AddModelError(nameof(consulta.DataHora),
                "Horário indisponível: outra pessoa já reservou esse horário com este médico. Escolha outro.");
    }

    /// <summary>Salva tratando a violação do índice único (corrida entre dois agendamentos).</summary>
    private async Task<bool> SalvarComTratamentoDeConflitoAsync(Consulta consulta)
    {
        try
        {
            await _db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(consulta.DataHora),
                "Horário indisponível: acabou de ser reservado por outra pessoa. Escolha outro.");
            await PrepararFormularioAsync(consulta.MedicoId);
            return false;
        }
    }

    /// <summary>Popula o &lt;select&gt; de médicos e a lista de feriados para o calendário.</summary>
    private async Task PrepararFormularioAsync(int? medicoSelecionado = null)
    {
        var medicos = await _db.Medicos
            .OrderBy(m => m.Especialidade).ThenBy(m => m.Nome)
            .ToListAsync();

        var grupos = medicos
            .Select(m => m.Especialidade)
            .Distinct()
            .ToDictionary(e => e, e => new SelectListGroup { Name = e });

        ViewBag.Medicos = medicos.Select(m => new SelectListItem
        {
            Value = m.Id.ToString(),
            Text = $"Dr(a). {m.Nome} · {m.Crm}",
            Group = grupos[m.Especialidade],
            Selected = medicoSelecionado.HasValue && m.Id == medicoSelecionado.Value
        }).ToList();

        int anoBase = DateTime.Today.Year;
        ViewBag.FeriadosJson = Enumerable.Range(anoBase, 3)
            .SelectMany(a => Agenda.FeriadosNacionais(a))
            .Select(f => f.Data.ToString("yyyy-MM-dd"))
            .Distinct()
            .ToList();
    }
}

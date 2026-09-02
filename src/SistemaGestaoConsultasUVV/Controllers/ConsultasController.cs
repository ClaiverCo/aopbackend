using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaGestaoConsultasUVV.Data;
using SistemaGestaoConsultasUVV.Models;

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

    // GET: /Consultas
    public async Task<IActionResult> Index()
    {
        var consultas = await _db.Consultas
            .Include(c => c.Medico)
            .Where(c => c.UsuarioId == UsuarioAtualId)
            .OrderBy(c => c.DataHora)
            .ToListAsync();
        return View(consultas);
    }

    // GET: /Consultas/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var consulta = await BuscarDoUsuarioAsync(id.Value);
        return consulta is null ? NotFound() : View(consulta);
    }

    // GET: /Consultas/Create
    public async Task<IActionResult> Create()
    {
        await PopularMedicosAsync();
        return View(new Consulta { DataHora = DateTime.Now.AddDays(1) });
    }

    // POST: /Consultas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MedicoId,DataHora,Descricao")] Consulta consulta)
    {
        await AplicarMedicoAsync(consulta);

        if (!ModelState.IsValid)
        {
            await PopularMedicosAsync(consulta.MedicoId);
            return View(consulta);
        }

        consulta.UsuarioId = UsuarioAtualId; // dono vem sempre das claims, nunca do form
        _db.Consultas.Add(consulta);
        await _db.SaveChangesAsync();

        TempData["Sucesso"] = "Consulta agendada com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Consultas/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var consulta = await BuscarDoUsuarioAsync(id.Value);
        if (consulta is null) return NotFound();

        await PopularMedicosAsync(consulta.MedicoId);
        return View(consulta);
    }

    // POST: /Consultas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,MedicoId,DataHora,Descricao")] Consulta consulta)
    {
        if (id != consulta.Id) return NotFound();

        var original = await BuscarDoUsuarioAsync(id);
        if (original is null) return NotFound();

        await AplicarMedicoAsync(consulta);

        if (!ModelState.IsValid)
        {
            await PopularMedicosAsync(consulta.MedicoId);
            return View(consulta);
        }

        original.MedicoId = consulta.MedicoId;
        original.Especialidade = consulta.Especialidade;
        original.DataHora = consulta.DataHora;
        original.Descricao = consulta.Descricao;
        await _db.SaveChangesAsync();

        TempData["Sucesso"] = "Consulta atualizada.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Consultas/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var consulta = await BuscarDoUsuarioAsync(id.Value);
        return consulta is null ? NotFound() : View(consulta);
    }

    // POST: /Consultas/Delete/5
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
        ModelState.Remove(nameof(consulta.Especialidade));
    }

    /// <summary>Monta a lista do &lt;select&gt; de médicos agrupada por especialidade.</summary>
    private async Task PopularMedicosAsync(int? selecionado = null)
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
            Selected = selecionado.HasValue && m.Id == selecionado.Value
        }).ToList();
    }
}

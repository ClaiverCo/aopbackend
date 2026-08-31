using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public IActionResult Create() =>
        View(new Consulta { DataHora = DateTime.Now.AddDays(1) });

    // POST: /Consultas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Especialidade,DataHora,Descricao")] Consulta consulta)
    {
        if (!ModelState.IsValid)
            return View(consulta);

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
        return consulta is null ? NotFound() : View(consulta);
    }

    // POST: /Consultas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Especialidade,DataHora,Descricao")] Consulta consulta)
    {
        if (id != consulta.Id) return NotFound();

        var original = await BuscarDoUsuarioAsync(id);
        if (original is null) return NotFound();

        if (!ModelState.IsValid)
            return View(consulta);

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
        _db.Consultas.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == UsuarioAtualId);
}

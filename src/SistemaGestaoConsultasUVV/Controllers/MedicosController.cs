using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestaoConsultasUVV.Data;

namespace SistemaGestaoConsultasUVV.Controllers;

/// <summary>
/// Corpo clínico (somente leitura). Os médicos são pré-cadastrados via seed do
/// EF Core e apenas consultados aqui e no formulário de agendamento.
/// </summary>
[Authorize]
public class MedicosController : Controller
{
    private readonly AppDbContext _db;

    public MedicosController(AppDbContext db) => _db = db;

    // GET: /Medicos
    public async Task<IActionResult> Index()
    {
        var medicos = await _db.Medicos
            .OrderBy(m => m.Especialidade).ThenBy(m => m.Nome)
            .ToListAsync();

        var agora = DateTime.Now;
        var reservados = await _db.Consultas
            .Where(c => c.DataHora >= agora)
            .OrderBy(c => c.DataHora)
            .Select(c => new { c.MedicoId, c.DataHora })
            .ToListAsync();

        // Próximos horários já reservados por médico (para exibir "indisponível").
        ViewBag.ProximosReservados = reservados
            .GroupBy(x => x.MedicoId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.DataHora).Take(4).ToList());

        return View(medicos);
    }
}

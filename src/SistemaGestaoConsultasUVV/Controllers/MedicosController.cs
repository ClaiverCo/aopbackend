using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestaoConsultasUVV.Data;

namespace SistemaGestaoConsultasUVV.Controllers;

/// <summary>
/// Corpo clínico (somente leitura). Os médicos são pré-cadastrados via seed do
/// EF Core e apenas consultados aqui e no formulário de agendamento — onde o
/// calendário mostra os horários livres (verde) e indisponíveis (vermelho).
/// </summary>
[Authorize]
public class MedicosController : Controller
{
    private readonly AppDbContext _db;

    public MedicosController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var medicos = await _db.Medicos
            .OrderBy(m => m.Especialidade).ThenBy(m => m.Nome)
            .ToListAsync();
        return View(medicos);
    }
}

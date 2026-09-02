using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SistemaGestaoConsultasUVV.Models;

/// <summary>
/// Profissional do corpo clínico. É pré-cadastrado via seed (HasData) e apenas
/// selecionado pelo usuário na hora de agendar uma consulta.
/// </summary>
public class Medico
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Médico")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Especialidade")]
    public string Especialidade { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Display(Name = "CRM")]
    public string Crm { get; set; } = string.Empty;

    /// <summary>Breve descrição da área de atuação (exibida no corpo clínico).</summary>
    [StringLength(300)]
    public string Resumo { get; set; } = string.Empty;

    [ValidateNever]
    public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();

    /// <summary>Rótulo pronto para listas/telas: "Dr(a). Nome — Especialidade".</summary>
    public string NomeExibicao => $"Dr(a). {Nome} — {Especialidade}";
}

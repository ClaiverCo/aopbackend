using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SistemaGestaoConsultasUVV.Models;

/// <summary>
/// Representa uma consulta médica/profissional agendada por um usuário.
/// </summary>
public class Consulta
{
    public int Id { get; set; }

    // Preenchida no servidor a partir da especialidade do médico escolhido —
    // não é digitada pelo usuário (por isso [ValidateNever]). Mantida na entidade
    // como exige o enunciado, com as Data Annotations de tamanho/obrigatoriedade.
    [Required(ErrorMessage = "A especialidade é obrigatória.")]
    [StringLength(100, ErrorMessage = "A especialidade deve ter no máximo 100 caracteres.")]
    [Display(Name = "Especialidade")]
    [ValidateNever]
    public string Especialidade { get; set; } = string.Empty;

    // Médico escolhido para a consulta (Relacionamento).
    [Display(Name = "Médico")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um médico.")]
    public int MedicoId { get; set; }

    [ForeignKey(nameof(MedicoId))]
    [ValidateNever]
    public Medico? Medico { get; set; }

    [Required(ErrorMessage = "A data/hora é obrigatória.")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    [Display(Name = "Data/Hora")]
    public DateTime DataHora { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = string.Empty;

    // Chave estrangeira para o usuário dono da consulta (Relacionamento).
    [Display(Name = "Usuário")]
    public int UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    [ValidateNever]
    public Usuario? Usuario { get; set; }
}

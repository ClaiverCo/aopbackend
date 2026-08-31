using System.ComponentModel.DataAnnotations;

namespace SistemaGestaoConsultasUVV.ViewModels;

/// <summary>Dados do formulário de login.</summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Senha { get; set; } = string.Empty;

    [Display(Name = "Manter-me conectado")]
    public bool Lembrar { get; set; }
}

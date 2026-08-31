using System.ComponentModel.DataAnnotations;

namespace SistemaGestaoConsultasUVV.Models;

/// <summary>
/// Representa um usuário do sistema. Abordagem Code First — o EF Core gera a tabela
/// "Usuarios" a partir desta classe.
/// </summary>
public class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(150)]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Armazena o HASH da senha (PBKDF2, gerado por <c>PasswordHasher&lt;Usuario&gt;</c>).
    /// A senha em texto puro nunca é persistida — ela existe apenas no
    /// <see cref="SistemaGestaoConsultasUVV.ViewModels.RegistroViewModel"/> durante o cadastro.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Senha { get; set; } = string.Empty;

    [Display(Name = "Data de Cadastro")]
    [DataType(DataType.DateTime)]
    public DateTime DataCadastro { get; set; }

    // Relacionamento 1-N: um usuário possui várias consultas.
    public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
}

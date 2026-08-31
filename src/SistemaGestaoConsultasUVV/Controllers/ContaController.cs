using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestaoConsultasUVV.Data;
using SistemaGestaoConsultasUVV.Models;
using SistemaGestaoConsultasUVV.ViewModels;

namespace SistemaGestaoConsultasUVV.Controllers;

/// <summary>
/// Cadastro de usuários e autenticação (login/logout) — lógica de acesso customizada
/// sobre cookie authentication.
/// </summary>
public class ContaController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<Usuario> _hasher;

    public ContaController(AppDbContext db, IPasswordHasher<Usuario> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    // -------------------- Cadastro (POST) --------------------

    [HttpGet]
    public IActionResult Registro() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(RegistroViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var emailNormalizado = model.Email.Trim().ToLowerInvariant();
        if (await _db.Usuarios.AnyAsync(u => u.Email == emailNormalizado))
        {
            ModelState.AddModelError(nameof(model.Email), "Já existe uma conta com esse e-mail.");
            return View(model);
        }

        var usuario = new Usuario
        {
            Nome = model.Nome.Trim(),
            Email = emailNormalizado,
            DataCadastro = DateTime.Now
        };
        // Nunca guardamos a senha em texto puro — apenas o hash.
        usuario.Senha = _hasher.HashPassword(usuario, model.Senha);

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        TempData["Sucesso"] = "Conta criada com sucesso! Faça login para continuar.";
        return RedirectToAction(nameof(Login));
    }

    // -------------------- Login --------------------

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid)
            return View(model);

        var emailNormalizado = model.Email.Trim().ToLowerInvariant();
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == emailNormalizado);

        if (usuario is null ||
            _hasher.VerifyHashedPassword(usuario, usuario.Senha, model.Senha) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = model.Lembrar });

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Consultas");
    }

    // -------------------- Logout --------------------

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AcessoNegado() => View();
}

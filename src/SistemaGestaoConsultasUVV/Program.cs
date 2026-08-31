using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaGestaoConsultasUVV.Api;
using SistemaGestaoConsultasUVV.Data;
using SistemaGestaoConsultasUVV.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Injeção de Dependência (contêiner de serviços)
// ---------------------------------------------------------------------------
builder.Services.AddControllersWithViews();

// EF Core + SQL Server. A connection string fica em appsettings.json.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Hash de senha (PBKDF2). Usamos apenas o utilitário do Identity, sem o schema completo.
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

// Autenticação baseada em cookie (implementação de login customizada).
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Conta/Login";
        options.LogoutPath = "/Conta/Logout";
        options.AccessDeniedPath = "/Conta/AcessoNegado";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;

        // Nas rotas de API respondemos com status HTTP (401/403) em vez de
        // redirecionar para a tela de login — facilita o teste via Swagger/Postman.
        options.Events.OnRedirectToLogin = ctx =>
        {
            if (ctx.Request.Path.StartsWithSegments("/api"))
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
            ctx.Response.Redirect(ctx.RedirectUri);
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = ctx =>
        {
            if (ctx.Request.Path.StartsWithSegments("/api"))
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
            ctx.Response.Redirect(ctx.RedirectUri);
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

// Swagger / OpenAPI para testar os endpoints de API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
    o.SwaggerDoc("v1", new() { Title = "API - Gestão de Consultas UVV", Version = "v1" }));

var app = builder.Build();

// Aplica as migrations pendentes automaticamente (conveniência para avaliação).
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

// ---------------------------------------------------------------------------
// Pipeline de Middleware — a ORDEM importa.
// ---------------------------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// IMPORTANTE: UseAuthentication() SEMPRE antes de UseAuthorization().
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Grupo de endpoints REST protegido — exige usuário autenticado.
app.MapGroup("/api/consultas")
   .WithTags("Consultas")
   .RequireAuthorization()
   .MapConsultasApi();

app.Run();

# Sistema de Gestão de Consultas UVV

Trabalho prático da disciplina de **Desenvolvimento Web Back-end** — Universidade Vila Velha (UVV).

Aplicação Web em **C# / ASP.NET Core MVC (.NET 10)** para gerenciamento de usuários e
registro de consultas médicas/profissionais, consolidando: arquitetura **MVC**,
**EF Core (Code First + Migrations)**, **segurança** (autenticação por cookie, senhas
com hash, rotas protegidas) e **protocolos de comunicação** (API REST + Swagger).

## Autor

- Claiver Corrêa dos Reis Ribeiro (trabalho individual)

## Vídeo demonstrativo

**Link:** _a adicionar_ <!-- TODO: colar aqui o link do vídeo (Loom / YouTube / similar) -->

O vídeo mostra o sistema funcionando: cadastro de usuário, login e registro de consulta.

## Repositório

<https://github.com/ClaiverCo/aopbackend>

---

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download) (`dotnet --version` → 10.x)
- **SQL Server** (Express, LocalDB ou instância completa)
- Ferramenta EF Core CLI:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

## 1. Configurar a Connection String

A string de conexão fica em [`src/SistemaGestaoConsultasUVV/appsettings.json`](src/SistemaGestaoConsultasUVV/appsettings.json),
na chave `ConnectionStrings:DefaultConnection`. O valor padrão aponta para o SQL Server Express local:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=SistemaGestaoConsultasUVV;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

Ajuste o `Server=` conforme o seu ambiente, por exemplo:

| Ambiente | Valor de `Server=` |
|----------|--------------------|
| SQL Server Express | `.\SQLEXPRESS` |
| LocalDB | `(localdb)\MSSQLLocalDB` |
| Instância padrão / Docker | `localhost` ou `localhost,1433` |

Se usar autenticação SQL em vez de Windows, troque `Trusted_Connection=True` por
`User Id=<usuario>;Password=<senha>`.

## 2. Criar o banco de dados (Migrations)

A partir da pasta do projeto:

```bash
cd src/SistemaGestaoConsultasUVV
dotnet ef database update
```

Isso executa a migration `InitialCreate` e cria o banco `SistemaGestaoConsultasUVV`
com as tabelas `Usuarios` e `Consultas`.

> Equivale ao `Update-Database` do **Package Manager Console** do Visual Studio.
> A aplicação também executa `Database.Migrate()` na inicialização, então rodar o
> `dotnet run` já com o banco vazio também funciona.

Comandos úteis:

```bash
# recriar a migration do zero
dotnet ef migrations add InitialCreate -o Migrations

# reverter o banco
dotnet ef database update 0
```

## 3. Executar a aplicação

```bash
cd src/SistemaGestaoConsultasUVV
dotnet run
```

- Aplicação MVC: <http://localhost:5224>
- Swagger (somente em ambiente Development): <http://localhost:5224/swagger>

## Fluxo de uso

1. **Cadastrar usuário** — `/Conta/Registro` (POST). Campos: Nome, E-mail, Senha, Confirmar Senha.
2. **Login** — `/Conta/Login`. Cria o cookie de autenticação.
3. **Consultas** — `/Consultas` (protegido por `[Authorize]`):
   - listar as consultas do usuário logado;
   - **Nova consulta** (`/Consultas/Create`): o usuário escolhe um **médico** num
     `<select>` agrupado por especialidade (ex.: agendar com o Dr(a). Anderson
     Vieira, em Cardiologia). A `Especialidade` da consulta é preenchida no
     servidor a partir do médico escolhido — não é digitada;
   - **Editar** (`/Consultas/Edit/{id}`) e **Excluir** (`/Consultas/Delete/{id}`).
   - Cada usuário só enxerga e altera as próprias consultas.
4. **Corpo Clínico** — `/Medicos`: lista os profissionais fictícios pré-cadastrados
   (seed do EF Core via `HasData`), agrupados pelas 7 especialidades — Clínica
   Médica, Pediatria, Ginecologia e Obstetrícia, Cardiologia, Ortopedia e
   Traumatologia, Dermatologia e Oftalmologia.

### Calendário de disponibilidade

Na tela de **Nova consulta / Editar**, depois de escolher o médico, um calendário
mostra a disponibilidade:

- **dias** — verde = dia útil disponível; cinza = fim de semana, **feriado nacional**
  ou data passada (não clicáveis);
- ao clicar num dia, aparecem os **horários comerciais de 08:00 às 18:00** (grade de
  30 min): **verde = livre**, **vermelho = indisponível** (já reservado para aquele
  médico ou já passou). Clicar num horário verde o seleciona.

O calendário é alimentado pelo endpoint `GET /Consultas/Disponibilidade?medicoId=&data=`.
Os feriados (fixos + móveis derivados da Páscoa: Carnaval, Sexta-feira Santa,
Corpus Christi) são calculados em `Services/Agenda.cs`.

### Regras de horário (validadas também no servidor)

- **Um médico não pode ter duas consultas no mesmo horário** — índice único
  `(MedicoId, DataHora)` + verificação no servidor. Colisão é recusada com
  *"Horário indisponível…"* no formulário e **HTTP 409** na API.
- Grade de 30 min; só dias úteis (seg–sex, exceto feriados); só 08:00–18:00; nada no passado.
- A corrida entre dois agendamentos simultâneos é tratada capturando `DbUpdateException`.
- O banco já vem com **8 pacientes fictícios** (senha `Paciente@123`) e **42
  consultas reservadas** distribuídas entre as especialidades (seed do EF Core),
  então já existem horários vermelhos para testar.

Usuários anônimos que tentam acessar `/Consultas` são redirecionados para o login.

## Testando a API (Swagger / Postman)

Endpoints REST em `/api/consultas`, todos protegidos com `.RequireAuthorization()`:

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/consultas` | Lista as consultas do usuário autenticado |
| GET | `/api/consultas/{id}` | Detalha uma consulta |
| POST | `/api/consultas` | Cria uma consulta — **409** se o horário do médico estiver ocupado |
| PUT | `/api/consultas/{id}` | Atualiza uma consulta — **409** em colisão de horário |
| DELETE | `/api/consultas/{id}` | Remove uma consulta |

Corpo de `POST`/`PUT` (a especialidade vem do médico):

```json
{ "medicoId": 7, "dataHora": "2027-05-07T11:00:00", "descricao": "Consulta de rotina" }
```

Sem autenticação a API responde **401 Unauthorized**. Como a autenticação é por
**cookie**, para usar o botão *Try it out* do Swagger basta estar logado no app MVC
**no mesmo navegador** (o cookie é reaproveitado). No **Postman**, faça primeiro um
`POST /Conta/Login` (form-url-encoded: `Email`, `Senha`) com o cookie jar habilitado
e reutilize o cookie `.AspNetCore.Cookies` nas chamadas seguintes.

## Estrutura do projeto

```
src/SistemaGestaoConsultasUVV/
├── Program.cs                 # DI (DbContext, auth, Swagger) + pipeline de middleware
├── appsettings.json           # ConnectionStrings:DefaultConnection
├── Data/AppDbContext.cs       # DbSets, índices únicos, relações 1-N, seed (médicos, pacientes, consultas)
├── Models/
│   ├── Usuario.cs             # Nome, Email, Senha (hash), DataCadastro
│   ├── Consulta.cs            # Especialidade, DataHora, Descricao, UsuarioId, MedicoId + grade de slots
│   └── Medico.cs              # Nome, Especialidade, CRM, Resumo (14 registros via HasData)
├── ViewModels/                # RegistroViewModel, LoginViewModel
├── Controllers/
│   ├── ContaController.cs     # Registro, Login, Logout, AcessoNegado
│   ├── ConsultasController.cs # CRUD [Authorize] + Disponibilidade (JSON do calendário)
│   └── MedicosController.cs   # Corpo clínico (somente leitura)
├── Services/Agenda.cs         # Expediente 08–18, grade de 30 min, fins de semana e feriados
├── Api/ConsultasEndpoints.cs  # Minimal API REST (grupo /api/consultas)
├── Views/                     # Razor (Conta/*, Consultas/* + _SeletorHorario, Medicos/*, Home/*)
└── Migrations/                # InitialCreate, AddMedicos, HorariosReservadosESeed
```

## Decisões de arquitetura

- **MVC (Controllers + Views)** para as telas; **Minimal API** para o CRUD REST.
- **EF Core Code First**: o banco é gerado a partir das classes de modelo via Migrations.
  Via `HasData` entram, junto com a migration: 14 médicos fictícios (7 especialidades),
  8 pacientes fictícios e 42 consultas já reservadas.
- **Relacionamentos**: `Consulta` pertence a um `Usuario` (cascade) e a um `Medico`
  (restrict). A `Especialidade` da consulta é uma cópia da especialidade do médico,
  gravada no servidor — o formulário só expõe a escolha do médico.
- **Horário indisponível**: índice único `(MedicoId, DataHora)` no banco + checagem
  no servidor (MVC e API) impedem dois agendamentos do mesmo médico no mesmo horário;
  a corrida entre dois `POST` simultâneos é tratada capturando `DbUpdateException`.
- **Validação** com Data Annotations (`[Required]`, `[EmailAddress]`, `[StringLength]`,
  `[Compare]`) — validada no servidor e no cliente (`_ValidationScriptsPartial`).
- **Segurança**:
  - autenticação por **cookie** (`AddAuthentication().AddCookie()`), login implementado
    manualmente no `ContaController`;
  - a **senha nunca é armazenada em texto puro** — grava-se o hash **PBKDF2**
    (`PasswordHasher<Usuario>`); a propriedade `Usuario.Senha` guarda esse hash;
  - rotas de consulta protegidas com `[Authorize]` (MVC) e `.RequireAuthorization()` (API);
  - no `Program.cs`, **`app.UseAuthentication()` vem antes de `app.UseAuthorization()`**.
- **Injeção de dependência**: `AppDbContext`, `IPasswordHasher<Usuario>` e os serviços de
  autenticação/autorização são registrados no contêiner de DI em `Program.cs`.

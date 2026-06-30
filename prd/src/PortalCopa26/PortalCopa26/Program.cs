using Microsoft.EntityFrameworkCore;
using PortalCopa26.Components;
using PortalCopa26.Data;
using PortalCopa26.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Persistência: EF Core + SQLite. Usa DbContextFactory porque componentes
// Blazor Server podem viver além do escopo de uma requisição.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=portalcopa26.db";
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// Serviços de dados da Landing Page (isolam a UI do EF Core).
builder.Services.AddScoped<ILandingPageService, LandingPageService>();

// Serviço de dados da página de Jogos.
builder.Services.AddScoped<IJogosService, JogosService>();

// Serviço da página Grupos (classificação oficial + resultados oficiais).
builder.Services.AddScoped<IGruposService, GruposService>();

// Serviço do Simulador (classificação + persistência da simulação corrente).
builder.Services.AddScoped<ISimuladorService, SimuladorService>();

// Serviço da página Equipes (seleções, técnicos e elencos).
builder.Services.AddScoped<ISelecaoService, SelecaoService>();

// Serviço da página Ranking FIFA (posição, pontuação e grupo).
builder.Services.AddScoped<IRankingService, RankingService>();

// Interop reutilizável de gráficos (Chart.js via JSInterop).
builder.Services.AddScoped<ChartInterop>();

var app = builder.Build();

// Aplica migrations pendentes e carrega os dados iniciais (idempotente).
await DbInitializer.InitializeAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

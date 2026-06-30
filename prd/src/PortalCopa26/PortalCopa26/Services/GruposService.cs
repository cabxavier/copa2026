using Microsoft.EntityFrameworkCore;
using PortalCopa26.Data;
using PortalCopa26.Services.Dtos;

namespace PortalCopa26.Services;

/// <summary>
/// Implementação dos dados da página Grupos sobre EF Core + SQLite usando
/// <see cref="IDbContextFactory{TContext}"/> (contextos de vida curta) e DTOs.
/// A classificação reutiliza <see cref="CalculadoraClassificacao"/> alimentada
/// exclusivamente pelos resultados <b>oficiais</b> de <c>Jogo</c>; os resultados
/// do Simulador (<c>SimulacaoJogo</c>) não são consultados aqui.
/// </summary>
public class GruposService : IGruposService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public GruposService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<IReadOnlyList<GrupoOpcaoDto>> ObterGruposAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();

        return await db.Grupos
            .AsNoTracking()
            .OrderBy(g => g.Nome)
            .Select(g => new GrupoOpcaoDto(g.Id, g.Nome))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<SimuladorJogoDto>> ObterJogosDoGrupoAsync(int grupoId)
    {
        await using var db = await _factory.CreateDbContextAsync();

        return await db.Jogos
            .AsNoTracking()
            .Where(j => j.GrupoId == grupoId)
            .OrderBy(j => j.Data)
            .ThenBy(j => j.Id)
            .Select(j => new SimuladorJogoDto(
                j.Id,
                j.SelecaoMandanteId, j.SelecaoMandante!.Nome, j.SelecaoMandante.BandeiraUrl,
                j.SelecaoVisitanteId, j.SelecaoVisitante!.Nome, j.SelecaoVisitante.BandeiraUrl,
                // Placar oficial vindo da própria tabela Jogos.
                j.GolsMandante, j.GolsVisitante,
                j.Data))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ClassificacaoLinhaDto>> ObterClassificacaoAsync(int grupoId)
    {
        // Reutiliza a lógica pura de classificação, sem duplicá-la.
        var jogos = await ObterJogosDoGrupoAsync(grupoId);
        return CalculadoraClassificacao.Calcular(jogos);
    }

    public async Task SalvarResultadoAsync(int jogoId, int? golsMandante, int? golsVisitante)
    {
        await using var db = await _factory.CreateDbContextAsync();

        var jogo = await db.Jogos.FirstOrDefaultAsync(j => j.Id == jogoId);
        if (jogo is null) return;

        if (golsMandante is int gm && golsVisitante is int gv)
        {
            // Resultado oficial completo: grava normalizando para inteiros não negativos.
            jogo.GolsMandante = Math.Max(0, gm);
            jogo.GolsVisitante = Math.Max(0, gv);
        }
        else
        {
            // Placar incompleto: o jogo deixa de contar — remove o resultado oficial.
            jogo.GolsMandante = null;
            jogo.GolsVisitante = null;
        }

        await db.SaveChangesAsync();
    }
}

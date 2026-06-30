using Microsoft.EntityFrameworkCore;
using PortalCopa26.Data;
using PortalCopa26.Models;
using PortalCopa26.Services.Dtos;

namespace PortalCopa26.Services;

/// <summary>
/// Implementação do Simulador sobre EF Core + SQLite usando
/// <see cref="IDbContextFactory{TContext}"/> (contextos de vida curta) e DTOs.
/// Mantém uma única <see cref="Simulacao"/> corrente, identificada por um nome
/// sentinela interno que nunca é exposto à UI.
/// </summary>
public class SimuladorService : ISimuladorService
{
    /// <summary>Nome sentinela da simulação corrente (uso interno; satisfaz o NOT NULL).</summary>
    private const string NomeSimulacaoCorrente = "__corrente__";

    private readonly IDbContextFactory<AppDbContext> _factory;

    public SimuladorService(IDbContextFactory<AppDbContext> factory)
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

        var jogos = await db.Jogos
            .AsNoTracking()
            .Where(j => j.GrupoId == grupoId)
            .OrderBy(j => j.Data)
            .ThenBy(j => j.Id)
            .Select(j => new
            {
                j.Id,
                j.SelecaoMandanteId,
                MandanteNome = j.SelecaoMandante!.Nome,
                MandanteBandeira = j.SelecaoMandante.BandeiraUrl,
                j.SelecaoVisitanteId,
                VisitanteNome = j.SelecaoVisitante!.Nome,
                VisitanteBandeira = j.SelecaoVisitante.BandeiraUrl
            })
            .ToListAsync();

        var placares = await ObterPlacaresInternoAsync(db);

        return jogos.Select(j =>
        {
            placares.TryGetValue(j.Id, out var p);
            return new SimuladorJogoDto(
                j.Id,
                j.SelecaoMandanteId, j.MandanteNome, j.MandanteBandeira,
                j.SelecaoVisitanteId, j.VisitanteNome, j.VisitanteBandeira,
                p?.GolsMandante, p?.GolsVisitante);
        }).ToList();
    }

    public async Task<IReadOnlyList<SelecaoGrupoDto>> ObterSelecoesDoGrupoAsync(int grupoId)
    {
        await using var db = await _factory.CreateDbContextAsync();

        return await db.Selecoes
            .AsNoTracking()
            .Where(s => s.GrupoId == grupoId)
            .OrderBy(s => s.Nome)
            .Select(s => new SelecaoGrupoDto(
                s.Nome,
                s.BandeiraUrl,
                s.RankingFifa != null ? s.RankingFifa.Posicao : (int?)null))
            .ToListAsync();
    }

    public async Task<IReadOnlyDictionary<int, (int GolsMandante, int GolsVisitante)>> ObterPlacaresAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();

        var placares = await ObterPlacaresInternoAsync(db);
        return placares.ToDictionary(
            kv => kv.Key,
            kv => (kv.Value.GolsMandante, kv.Value.GolsVisitante));
    }

    public async Task<IReadOnlyList<ClassificacaoLinhaDto>> ObterClassificacaoAsync(int grupoId)
    {
        var jogos = await ObterJogosDoGrupoAsync(grupoId);
        return CalculadoraClassificacao.Calcular(jogos);
    }

    public async Task SalvarPlacarAsync(int jogoId, int? golsMandante, int? golsVisitante)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var sim = await ObterOuCriarSimulacaoCorrenteAsync(db, criarSeAusente: true);

        var existente = await db.SimulacaoJogos
            .FirstOrDefaultAsync(sj => sj.SimulacaoId == sim!.Id && sj.JogoId == jogoId);

        if (golsMandante is int gm && golsVisitante is int gv)
        {
            if (existente is null)
            {
                db.SimulacaoJogos.Add(new SimulacaoJogo
                {
                    SimulacaoId = sim!.Id,
                    JogoId = jogoId,
                    GolsMandante = gm,
                    GolsVisitante = gv
                });
            }
            else
            {
                existente.GolsMandante = gm;
                existente.GolsVisitante = gv;
            }
        }
        else if (existente is not null)
        {
            // Placar incompleto: o jogo deixa de contar — remove o resultado.
            db.SimulacaoJogos.Remove(existente);
        }

        await db.SaveChangesAsync();
    }

    public async Task LimparGrupoAsync(int grupoId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var sim = await ObterOuCriarSimulacaoCorrenteAsync(db, criarSeAusente: false);
        if (sim is null) return;

        var jogoIds = await db.Jogos
            .Where(j => j.GrupoId == grupoId)
            .Select(j => j.Id)
            .ToListAsync();

        var alvos = await db.SimulacaoJogos
            .Where(sj => sj.SimulacaoId == sim.Id && jogoIds.Contains(sj.JogoId))
            .ToListAsync();

        db.SimulacaoJogos.RemoveRange(alvos);
        await db.SaveChangesAsync();
    }

    public async Task LimparTudoAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        var sim = await ObterOuCriarSimulacaoCorrenteAsync(db, criarSeAusente: false);
        if (sim is null) return;

        var alvos = await db.SimulacaoJogos
            .Where(sj => sj.SimulacaoId == sim.Id)
            .ToListAsync();

        db.SimulacaoJogos.RemoveRange(alvos);
        await db.SaveChangesAsync();
    }

    private async Task<Dictionary<int, SimulacaoJogo>> ObterPlacaresInternoAsync(AppDbContext db)
    {
        var sim = await ObterOuCriarSimulacaoCorrenteAsync(db, criarSeAusente: false);
        if (sim is null)
            return new Dictionary<int, SimulacaoJogo>();

        return await db.SimulacaoJogos
            .AsNoTracking()
            .Where(sj => sj.SimulacaoId == sim.Id)
            .ToDictionaryAsync(sj => sj.JogoId);
    }

    private static async Task<Simulacao?> ObterOuCriarSimulacaoCorrenteAsync(AppDbContext db, bool criarSeAusente)
    {
        var sim = await db.Simulacoes
            .FirstOrDefaultAsync(s => s.Nome == NomeSimulacaoCorrente);

        if (sim is null && criarSeAusente)
        {
            sim = new Simulacao
            {
                Nome = NomeSimulacaoCorrente,
                DataCriacao = DateTime.UtcNow
            };
            db.Simulacoes.Add(sim);
            await db.SaveChangesAsync();
        }

        return sim;
    }
}

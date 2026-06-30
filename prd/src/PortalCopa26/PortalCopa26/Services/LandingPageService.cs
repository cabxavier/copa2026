using Microsoft.EntityFrameworkCore;
using PortalCopa26.Data;
using PortalCopa26.Data.Seed;
using PortalCopa26.Services.Dtos;

namespace PortalCopa26.Services;

/// <summary>
/// Implementação dos dados da Landing Page sobre EF Core + SQLite.
/// Usa <see cref="IDbContextFactory{TContext}"/> (contextos de vida curta) e
/// retorna DTOs, sem expor entidades à UI.
/// </summary>
public class LandingPageService : ILandingPageService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public LandingPageService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<EstatisticasCopaDto> ObterEstatisticasAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();

        // Seleções e grupos refletem o banco; estádios e total de jogos são os
        // números oficiais do torneio (de /fontes), pois o mata-mata não é semeado.
        var selecoes = await db.Selecoes.CountAsync();
        var grupos = await db.Grupos.CountAsync();

        return new EstatisticasCopaDto(
            Selecoes: selecoes,
            Grupos: grupos,
            Estadios: DadosCopa.TotalEstadios,
            Jogos: DadosCopa.TotalJogos);
    }

    public Task<IReadOnlyList<PaisSedeDto>> ObterPaisesSedeAsync()
    {
        IReadOnlyList<PaisSedeDto> paises = DadosCopa.PaisesSede
            .Select(p => new PaisSedeDto(p.Pais, p.Codigo, p.Estadios))
            .ToList();

        return Task.FromResult(paises);
    }

    public async Task<IReadOnlyList<JogoResumoDto>> ObterProximosJogosAsync(int quantidade = 6)
    {
        await using var db = await _factory.CreateDbContextAsync();

        var hoje = DateTime.UtcNow.Date;

        var query = db.Jogos
            .AsNoTracking()
            .Include(j => j.SelecaoMandante)
            .Include(j => j.SelecaoVisitante)
            .Include(j => j.Grupo)
            .OrderBy(j => j.Data)
            .AsQueryable();

        // Prioriza jogos a partir de hoje; se não houver futuros, mostra os primeiros.
        var futuros = await ProjetarAsync(query.Where(j => j.Data >= hoje), quantidade);
        if (futuros.Count > 0)
        {
            return futuros;
        }

        return await ProjetarAsync(query, quantidade);
    }

    public async Task<IReadOnlyList<RankingItemDto>> ObterRankingTopAsync(int n = 10)
    {
        await using var db = await _factory.CreateDbContextAsync();

        return await db.RankingFifa
            .AsNoTracking()
            .Include(r => r.Selecao)
            .OrderBy(r => r.Posicao)
            .Take(n)
            .Select(r => new RankingItemDto(
                r.Posicao,
                r.Selecao!.Nome,
                r.Selecao.Codigo,
                r.Pontuacao))
            .ToListAsync();
    }

    private static async Task<IReadOnlyList<JogoResumoDto>> ProjetarAsync(
        IQueryable<Models.Jogo> query, int quantidade)
    {
        return await query
            .Take(quantidade)
            .Select(j => new JogoResumoDto(
                j.Id,
                j.Data,
                j.Grupo != null ? j.Grupo.Nome : string.Empty,
                j.SelecaoMandante!.Nome,
                j.SelecaoMandante.BandeiraUrl,
                j.SelecaoVisitante!.Nome,
                j.SelecaoVisitante.BandeiraUrl,
                j.Estadio,
                j.Cidade,
                // Placar oficial persistido na tabela Jogos (gravado pela página Grupos).
                j.GolsMandante,
                j.GolsVisitante))
            .ToListAsync();
    }
}

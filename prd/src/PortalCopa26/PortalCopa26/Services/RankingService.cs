using Microsoft.EntityFrameworkCore;
using PortalCopa26.Data;
using PortalCopa26.Services.Dtos;

namespace PortalCopa26.Services;

/// <summary>
/// Implementação dos dados da página Ranking sobre EF Core + SQLite usando
/// <see cref="IDbContextFactory{TContext}"/> (contextos de vida curta) e DTOs.
/// O ranking vem exclusivamente da tabela <c>RankingFifa</c> (seed de
/// copa2026_ranking_fifa.txt); o grupo é obtido via <c>Selecao.Grupo</c>.
/// </summary>
public class RankingService : IRankingService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public RankingService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<IReadOnlyList<RankingFifaItemDto>> ObterRankingAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();

        return await db.RankingFifa
            .AsNoTracking()
            .Include(r => r.Selecao)
                .ThenInclude(s => s!.Grupo)
            .OrderBy(r => r.Posicao)
            .Select(r => new RankingFifaItemDto(
                r.Posicao,
                r.Selecao!.Nome,
                r.Selecao.Codigo,
                r.Selecao.BandeiraUrl,
                r.Pontuacao,
                r.Selecao.Grupo!.Nome))
            .ToListAsync();
    }
}

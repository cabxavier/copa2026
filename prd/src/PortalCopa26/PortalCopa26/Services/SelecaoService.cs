using System.Globalization;
using Microsoft.EntityFrameworkCore;
using PortalCopa26.Data;
using PortalCopa26.Services.Dtos;

namespace PortalCopa26.Services;

/// <summary>
/// Implementação dos dados da página Equipes sobre EF Core + SQLite usando
/// <see cref="IDbContextFactory{TContext}"/> (contextos de vida curta) e DTOs.
/// A ordenação alfabética das seleções é feita em memória com cultura pt-BR,
/// pois o SQLite não aplica comparação culture-aware (acentuação) no servidor.
/// </summary>
public class SelecaoService : ISelecaoService
{
    private static readonly StringComparer ComparadorPtBr =
        StringComparer.Create(CultureInfo.GetCultureInfo("pt-BR"), ignoreCase: false);

    private readonly IDbContextFactory<AppDbContext> _factory;

    public SelecaoService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<IReadOnlyList<SelecaoCardDto>> ObterSelecoesAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();

        var selecoes = await db.Selecoes
            .AsNoTracking()
            .Select(s => new SelecaoCardDto(
                s.Id,
                s.Nome,
                s.Codigo,
                s.BandeiraUrl,
                s.GrupoId,
                s.Grupo!.Nome))
            .ToListAsync();

        // Ordenação culture-aware (pt-BR): trata "África", "Áustria", "Argélia".
        return selecoes
            .OrderBy(s => s.Nome, ComparadorPtBr)
            .ToList();
    }

    public async Task<SelecaoDetalheDto?> ObterDetalheAsync(int selecaoId)
    {
        await using var db = await _factory.CreateDbContextAsync();

        var selecao = await db.Selecoes
            .AsNoTracking()
            .Where(s => s.Id == selecaoId)
            .Select(s => new
            {
                s.Id,
                s.Nome,
                s.Codigo,
                s.BandeiraUrl,
                GrupoNome = s.Grupo!.Nome,
                s.Tecnico,
                RankingPosicao = (int?)(s.RankingFifa != null ? s.RankingFifa.Posicao : (int?)null),
                Jogadores = s.Jogadores
                    .OrderBy(j => j.Posicao)
                    .ThenBy(j => j.Nome)
                    .Select(j => new JogadorDto(j.Nome, j.Posicao, j.Idade, j.GolsMarcados))
                    .ToList(),
            })
            .FirstOrDefaultAsync();

        if (selecao is null) return null;

        return new SelecaoDetalheDto(
            selecao.Id,
            selecao.Nome,
            selecao.Codigo,
            selecao.BandeiraUrl,
            selecao.GrupoNome,
            selecao.Tecnico,
            selecao.RankingPosicao,
            selecao.Jogadores);
    }
}

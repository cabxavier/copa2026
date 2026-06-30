using PortalCopa26.Services.Dtos;

namespace PortalCopa26.Services;

/// <summary>
/// Dados da página Ranking FIFA. Fonte exclusiva: o ranking oficial persistido
/// (seed de copa2026_ranking_fifa.txt). A UI consome apenas DTOs; nenhum
/// componente acessa o <c>DbContext</c> diretamente.
/// </summary>
public interface IRankingService
{
    /// <summary>
    /// Ranking FIFA das seleções da Copa, ordenado crescentemente pela posição,
    /// já com a pontuação e o grupo de cada seleção.
    /// </summary>
    Task<IReadOnlyList<RankingFifaItemDto>> ObterRankingAsync();
}

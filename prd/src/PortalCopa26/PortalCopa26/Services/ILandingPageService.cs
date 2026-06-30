using PortalCopa26.Services.Dtos;

namespace PortalCopa26.Services;

/// <summary>
/// Fornece os dados da Landing Page. Isola os componentes do EF Core.
/// </summary>
public interface ILandingPageService
{
    Task<EstatisticasCopaDto> ObterEstatisticasAsync();

    Task<IReadOnlyList<PaisSedeDto>> ObterPaisesSedeAsync();

    Task<IReadOnlyList<JogoResumoDto>> ObterProximosJogosAsync(int quantidade = 6);

    Task<IReadOnlyList<RankingItemDto>> ObterRankingTopAsync(int n = 10);
}

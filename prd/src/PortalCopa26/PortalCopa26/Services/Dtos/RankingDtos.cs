using System.Globalization;

namespace PortalCopa26.Services.Dtos;

/// <summary>
/// Item do ranking FIFA exibido na página Ranking. Inclui o grupo da seleção,
/// diferindo de <see cref="RankingItemDto"/> (usado apenas no gráfico da Landing
/// Page). A pontuação vem fielmente da fonte oficial (copa2026_ranking_fifa.txt).
/// </summary>
public record RankingFifaItemDto(
    int Posicao,
    string Selecao,
    string Codigo,
    string? BandeiraUrl,
    decimal Pontuacao,
    string Grupo)
{
    private static readonly CultureInfo CulturaPtBr = CultureInfo.GetCultureInfo("pt-BR");

    /// <summary>Pontuação formatada com duas casas decimais em pt-BR (ex.: "1877,72").</summary>
    public string PontuacaoFormatada => Pontuacao.ToString("N2", CulturaPtBr);

    /// <summary>Letra do grupo (ex.: "Grupo A" → "A") para o rótulo "GRP".</summary>
    public string GrupoLetra => Grupo.Replace("Grupo ", string.Empty);

    /// <summary>Indica se a seleção está no pódio (três primeiras posições).</summary>
    public bool EhTop3 => Posicao is >= 1 and <= 3;
}

namespace PortalCopa26.Models;

/// <summary>
/// Posição e pontuação de uma seleção no ranking FIFA.
/// </summary>
public class RankingFifa
{
    public int Id { get; set; }

    public int SelecaoId { get; set; }
    public Selecao? Selecao { get; set; }

    public decimal Pontuacao { get; set; }

    public int Posicao { get; set; }
}

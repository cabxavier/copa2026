namespace PortalCopa26.Models;

/// <summary>
/// Jogador integrante do elenco de uma seleção.
/// </summary>
public class Jogador
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public PosicaoJogador Posicao { get; set; }

    public int Idade { get; set; }

    public int GolsMarcados { get; set; }

    /// <summary>Número de Copas do Mundo das quais o jogador participou.</summary>
    public int ParticipacoesCopas { get; set; }

    public int SelecaoId { get; set; }
    public Selecao? Selecao { get; set; }
}

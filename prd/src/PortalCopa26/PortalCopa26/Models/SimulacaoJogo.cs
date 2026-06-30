namespace PortalCopa26.Models;

/// <summary>
/// Resultado simulado de um jogo dentro de uma simulação. Referencia o
/// <see cref="Jogo"/> oficial sem alterá-lo.
/// </summary>
public class SimulacaoJogo
{
    public int Id { get; set; }

    public int SimulacaoId { get; set; }
    public Simulacao? Simulacao { get; set; }

    public int JogoId { get; set; }
    public Jogo? Jogo { get; set; }

    public int GolsMandante { get; set; }

    public int GolsVisitante { get; set; }
}

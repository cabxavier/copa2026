namespace PortalCopa26.Models;

/// <summary>
/// Partida oficial da Copa do Mundo 2026. O placar oficial é opcional
/// (preenchido somente após a realização do jogo).
/// </summary>
public class Jogo
{
    public int Id { get; set; }

    public DateTime Data { get; set; }

    public int SelecaoMandanteId { get; set; }
    public Selecao? SelecaoMandante { get; set; }

    public int SelecaoVisitanteId { get; set; }
    public Selecao? SelecaoVisitante { get; set; }

    public int? GrupoId { get; set; }
    public Grupo? Grupo { get; set; }

    public string Estadio { get; set; } = string.Empty;

    public string Cidade { get; set; } = string.Empty;

    /// <summary>Gols do mandante no resultado oficial (nulo até o jogo ocorrer).</summary>
    public int? GolsMandante { get; set; }

    /// <summary>Gols do visitante no resultado oficial (nulo até o jogo ocorrer).</summary>
    public int? GolsVisitante { get; set; }
}

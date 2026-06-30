namespace PortalCopa26.Models;

/// <summary>
/// Grupo da fase de grupos da Copa do Mundo 2026 (ex.: "Grupo A").
/// </summary>
public class Grupo
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public ICollection<Selecao> Selecoes { get; set; } = new List<Selecao>();

    public ICollection<Jogo> Jogos { get; set; } = new List<Jogo>();
}

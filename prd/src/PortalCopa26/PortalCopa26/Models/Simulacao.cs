namespace PortalCopa26.Models;

/// <summary>
/// Sessão de simulação criada por um usuário. Persiste entre execuções da aplicação.
/// </summary>
public class Simulacao
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public DateTime DataCriacao { get; set; }

    public ICollection<SimulacaoJogo> Jogos { get; set; } = new List<SimulacaoJogo>();
}

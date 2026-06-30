namespace PortalCopa26.Models;

/// <summary>
/// Seleção nacional participante da Copa do Mundo 2026.
/// </summary>
public class Selecao
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    /// <summary>Código ISO de 3 letras (ex.: "BRA").</summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>URL da bandeira (padrão da API pública da FIFA).</summary>
    public string? BandeiraUrl { get; set; }

    /// <summary>Nome do técnico da seleção (origem: copa2026_pais_tecnicos.txt).</summary>
    public string? Tecnico { get; set; }

    public int GrupoId { get; set; }
    public Grupo? Grupo { get; set; }

    public ICollection<Jogador> Jogadores { get; set; } = new List<Jogador>();

    public RankingFifa? RankingFifa { get; set; }
}

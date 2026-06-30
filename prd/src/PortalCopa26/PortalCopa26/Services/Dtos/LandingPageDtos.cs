namespace PortalCopa26.Services.Dtos;

/// <summary>Números oficiais do torneio exibidos no card de estatísticas.</summary>
public record EstatisticasCopaDto(int Selecoes, int Grupos, int Estadios, int Jogos);

/// <summary>País-sede com a quantidade de estádios.</summary>
public record PaisSedeDto(string Pais, string Codigo, int Estadios);

/// <summary>Resumo de um jogo para listagem (próximos jogos).</summary>
public record JogoResumoDto(
    int Id,
    DateTime Data,
    string Grupo,
    string MandanteNome,
    string? MandanteBandeira,
    string VisitanteNome,
    string? VisitanteBandeira,
    string Estadio,
    string Cidade,
    int? GolsMandante,
    int? GolsVisitante)
{
    /// <summary>Há placar oficial registrado (ambos os gols informados).</summary>
    public bool TemResultado => GolsMandante is not null && GolsVisitante is not null;
}

/// <summary>Item do ranking FIFA para o gráfico.</summary>
public record RankingItemDto(int Posicao, string Selecao, string Codigo, decimal Pontuacao);

namespace PortalCopa26.Services.Dtos;

/// <summary>Jogo da fase de grupos para a listagem da página de Jogos.</summary>
public record JogoListaDto(
    int Id,
    DateTime Data,
    string GrupoNome,
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

/// <summary>Opção de grupo para o filtro da página de Jogos (rótulo = Grupo.Nome).</summary>
public record GrupoOpcaoDto(int Id, string Nome);

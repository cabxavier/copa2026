using PortalCopa26.Models;

namespace PortalCopa26.Services.Dtos;

/// <summary>Seleção na grade da página Equipes (card).</summary>
public record SelecaoCardDto(
    int Id,
    string Nome,
    string Codigo,
    string? BandeiraUrl,
    int GrupoId,
    string GrupoNome);

/// <summary>Detalhe de uma seleção para o modal de elenco.</summary>
public record SelecaoDetalheDto(
    int Id,
    string Nome,
    string Codigo,
    string? BandeiraUrl,
    string GrupoNome,
    string? Tecnico,
    int? RankingPosicao,
    IReadOnlyList<JogadorDto> Jogadores);

/// <summary>Jogador convocado exibido na tabela de elenco.</summary>
public record JogadorDto(string Nome, PosicaoJogador Posicao, int Idade, int Gols)
{
    /// <summary>Classe CSS da etiqueta de posição (pos-GK/DF/MF/FW).</summary>
    public string PosicaoCss => Posicao switch
    {
        PosicaoJogador.Goleiro => "pos-GK",
        PosicaoJogador.Defensor => "pos-DF",
        PosicaoJogador.MeioCampo => "pos-MF",
        PosicaoJogador.Atacante => "pos-FW",
        _ => "pos-MF",
    };

    /// <summary>Sigla exibida na etiqueta de posição.</summary>
    public string PosicaoSigla => Posicao switch
    {
        PosicaoJogador.Goleiro => "GOL",
        PosicaoJogador.Defensor => "DEF",
        PosicaoJogador.MeioCampo => "MEI",
        PosicaoJogador.Atacante => "ATA",
        _ => "MEI",
    };
}

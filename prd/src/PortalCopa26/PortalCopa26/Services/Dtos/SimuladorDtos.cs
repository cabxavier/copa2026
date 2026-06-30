namespace PortalCopa26.Services.Dtos;

/// <summary>
/// Jogo da fase de grupos com placar opcional (nulo enquanto não informado).
/// Representa genericamente "um jogo de grupo com placar" e é reutilizado tanto
/// pelo Simulador (placar simulado) quanto pela página Grupos (placar oficial),
/// servindo de entrada para <see cref="CalculadoraClassificacao"/>.
/// Melhoria futura: renomear para um nome neutro (ex.: <c>JogoPlacarDto</c>).
/// Reutiliza <see cref="GrupoOpcaoDto"/> de <c>JogosDtos</c> para as opções de grupo.
/// </summary>
public record SimuladorJogoDto(
    int JogoId,
    int MandanteId,
    string MandanteNome,
    string? MandanteBandeira,
    int VisitanteId,
    string VisitanteNome,
    string? VisitanteBandeira,
    int? GolsMandante,
    int? GolsVisitante,
    // Data/horário oficial do jogo. Opcional: o Simulador não a utiliza; a
    // página Grupos a preenche para exibir a data na listagem de jogos.
    DateTime? Data = null);

/// <summary>Linha da classificação simulada de um grupo.</summary>
public record ClassificacaoLinhaDto(
    int Posicao,
    int SelecaoId,
    string Nome,
    string? Bandeira,
    int J,
    int V,
    int E,
    int D,
    int GolsPro,
    int GolsContra,
    int Saldo,
    int Pontos);

/// <summary>Seleção do grupo para o painel de resumo (nome, bandeira, ranking).</summary>
public record SelecaoGrupoDto(
    string Nome,
    string? Bandeira,
    int? RankingPosicao);

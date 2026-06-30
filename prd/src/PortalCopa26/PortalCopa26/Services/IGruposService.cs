using PortalCopa26.Services.Dtos;

namespace PortalCopa26.Services;

/// <summary>
/// Dados e persistência da página Grupos. A classificação é calculada a partir
/// dos resultados <b>oficiais</b> da tabela <c>Jogos</c> (independente do
/// Simulador). A UI consome apenas DTOs; nenhum componente acessa o
/// <c>DbContext</c> diretamente.
/// </summary>
public interface IGruposService
{
    /// <summary>Grupos oficiais (A–L) como opções para os chips de navegação.</summary>
    Task<IReadOnlyList<GrupoOpcaoDto>> ObterGruposAsync();

    /// <summary>
    /// Jogos do grupo já com os placares <b>oficiais</b> preenchidos
    /// (nulos quando ainda não informados), ordenados cronologicamente.
    /// </summary>
    Task<IReadOnlyList<SimuladorJogoDto>> ObterJogosDoGrupoAsync(int grupoId);

    /// <summary>Classificação oficial do grupo, calculada a partir dos resultados oficiais.</summary>
    Task<IReadOnlyList<ClassificacaoLinhaDto>> ObterClassificacaoAsync(int grupoId);

    /// <summary>
    /// Persiste o resultado <b>oficial</b> de um jogo na tabela <c>Jogos</c>.
    /// Com ambos os gols informados, grava o placar; se algum for nulo, o
    /// resultado oficial é removido (ambos voltam a nulo). Valores negativos
    /// são normalizados para 0.
    /// </summary>
    Task SalvarResultadoAsync(int jogoId, int? golsMandante, int? golsVisitante);
}

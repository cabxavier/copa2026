using PortalCopa26.Services.Dtos;

namespace PortalCopa26.Services;

/// <summary>
/// Dados e persistência do Simulador da fase de grupos. Mantém automaticamente
/// uma única simulação corrente (sem nome exigido do usuário). A UI consome
/// apenas DTOs; nenhum componente acessa o <c>DbContext</c> diretamente.
/// </summary>
public interface ISimuladorService
{
    /// <summary>Grupos oficiais (A–L) como opções para o seletor.</summary>
    Task<IReadOnlyList<GrupoOpcaoDto>> ObterGruposAsync();

    /// <summary>
    /// Jogos do grupo já com os placares da simulação corrente preenchidos
    /// (nulos quando ainda não informados), ordenados cronologicamente.
    /// </summary>
    Task<IReadOnlyList<SimuladorJogoDto>> ObterJogosDoGrupoAsync(int grupoId);

    /// <summary>Seleções do grupo (nome, bandeira e posição de ranking quando houver).</summary>
    Task<IReadOnlyList<SelecaoGrupoDto>> ObterSelecoesDoGrupoAsync(int grupoId);

    /// <summary>Placares da simulação corrente por <c>JogoId</c> (apenas jogos com resultado).</summary>
    Task<IReadOnlyDictionary<int, (int GolsMandante, int GolsVisitante)>> ObterPlacaresAsync();

    /// <summary>Classificação simulada do grupo a partir do estado persistido.</summary>
    Task<IReadOnlyList<ClassificacaoLinhaDto>> ObterClassificacaoAsync(int grupoId);

    /// <summary>
    /// Persiste o placar de um jogo na simulação corrente. Com ambos os gols,
    /// insere/atualiza o resultado; se algum for nulo, remove o resultado.
    /// </summary>
    Task SalvarPlacarAsync(int jogoId, int? golsMandante, int? golsVisitante);

    /// <summary>Zera os placares de todos os jogos do grupo na simulação corrente.</summary>
    Task LimparGrupoAsync(int grupoId);

    /// <summary>Zera todos os placares da simulação corrente.</summary>
    Task LimparTudoAsync();
}

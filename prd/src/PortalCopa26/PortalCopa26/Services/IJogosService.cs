using PortalCopa26.Services.Dtos;

namespace PortalCopa26.Services;

/// <summary>
/// Fornece os dados da página de Jogos. Isola os componentes do EF Core.
/// </summary>
public interface IJogosService
{
    /// <summary>Jogos da fase de grupos, opcionalmente filtrados por grupo, ordenados por data.</summary>
    Task<IReadOnlyList<JogoListaDto>> ObterJogosAsync(int? grupoId = null);

    /// <summary>Grupos oficiais (A–L) para o filtro.</summary>
    Task<IReadOnlyList<GrupoOpcaoDto>> ObterGruposAsync();
}

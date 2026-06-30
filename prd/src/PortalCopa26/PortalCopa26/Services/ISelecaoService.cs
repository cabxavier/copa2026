using PortalCopa26.Services.Dtos;

namespace PortalCopa26.Services;

/// <summary>
/// Dados da página Equipes (seleções e elencos). A UI consome apenas DTOs;
/// nenhum componente acessa o <c>DbContext</c> diretamente.
/// </summary>
public interface ISelecaoService
{
    /// <summary>
    /// As 48 seleções para a grade, ordenadas alfabeticamente por nome
    /// (cultura pt-BR, tratando acentuação corretamente).
    /// </summary>
    Task<IReadOnlyList<SelecaoCardDto>> ObterSelecoesAsync();

    /// <summary>
    /// Detalhe de uma seleção (técnico, ranking e elenco convocado) para o modal,
    /// ou <c>null</c> se a seleção não existir. O elenco vem ordenado por posição
    /// (Goleiro→Atacante) e, dentro da posição, por nome.
    /// </summary>
    Task<SelecaoDetalheDto?> ObterDetalheAsync(int selecaoId);
}

using Microsoft.EntityFrameworkCore;
using PortalCopa26.Data;
using PortalCopa26.Services.Dtos;

namespace PortalCopa26.Services;

/// <summary>
/// Implementação dos dados da página de Jogos sobre EF Core + SQLite.
/// Usa <see cref="IDbContextFactory{TContext}"/> (contextos de vida curta) e
/// retorna DTOs, sem expor entidades à UI.
/// </summary>
public class JogosService : IJogosService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public JogosService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<IReadOnlyList<JogoListaDto>> ObterJogosAsync(int? grupoId = null)
    {
        await using var db = await _factory.CreateDbContextAsync();

        var query = db.Jogos
            .AsNoTracking()
            .Include(j => j.SelecaoMandante)
            .Include(j => j.SelecaoVisitante)
            .Include(j => j.Grupo)
            .AsQueryable();

        if (grupoId is int id)
        {
            query = query.Where(j => j.GrupoId == id);
        }

        // Os dados oficiais não têm horário (data à meia-noite); o desempate por Id
        // garante uma ordem determinística e estável entre jogos do mesmo dia.
        return await query
            .OrderBy(j => j.Data)
            .ThenBy(j => j.Id)
            .Select(j => new JogoListaDto(
                j.Id,
                j.Data,
                j.Grupo != null ? j.Grupo.Nome : string.Empty,
                j.SelecaoMandante!.Nome,
                j.SelecaoMandante.BandeiraUrl,
                j.SelecaoVisitante!.Nome,
                j.SelecaoVisitante.BandeiraUrl,
                j.Estadio,
                j.Cidade,
                // Placar oficial persistido na tabela Jogos (gravado pela página Grupos).
                j.GolsMandante,
                j.GolsVisitante))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<GrupoOpcaoDto>> ObterGruposAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();

        return await db.Grupos
            .AsNoTracking()
            .OrderBy(g => g.Nome)
            .Select(g => new GrupoOpcaoDto(g.Id, g.Nome))
            .ToListAsync();
    }
}

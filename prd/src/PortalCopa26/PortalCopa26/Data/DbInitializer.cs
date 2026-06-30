using Microsoft.EntityFrameworkCore;
using PortalCopa26.Data.Seed;
using PortalCopa26.Models;

namespace PortalCopa26.Data;

/// <summary>
/// Aplica as migrations pendentes e popula os dados oficiais da Copa. Os dados
/// estruturais (grupos, seleções, ranking e jogos) vêm de <see cref="DadosCopa"/>
/// (transcritos de /fontes). Já os <b>elencos e técnicos</b> são lidos em runtime
/// dos arquivos oficiais <c>copa2026_selecoes_jogadores.txt</c> e
/// <c>copa2026_pais_tecnicos.txt</c> (copiados para a saída em <c>fontes/</c>),
/// pelo volume de registros. Cada etapa é idempotente e tem guarda própria, de
/// modo que elencos/técnicos populem inclusive bancos de desenvolvimento já criados.
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        await using var db = await factory.CreateDbContextAsync();

        // Garante o banco criado/atualizado conforme as migrations.
        await db.Database.MigrateAsync();

        await SeedDadosOficiaisAsync(db);

        // Elencos e técnicos têm guarda própria (independente de Grupos) para
        // popularem bancos já existentes onde as colunas/tabelas eram novidade.
        await SeedElencoETecnicosAsync(db);
    }

    private static async Task SeedDadosOficiaisAsync(AppDbContext db)
    {
        if (await db.Grupos.AnyAsync())
        {
            // Dados estruturais já carregados; nada a fazer (idempotência).
            return;
        }

        // 12 grupos (A–L).
        var grupos = DadosCopa.Grupos
            .Select(letra => new Grupo { Nome = $"Grupo {letra}" })
            .ToList();
        db.Grupos.AddRange(grupos);
        await db.SaveChangesAsync();

        var grupoPorLetra = grupos.ToDictionary(g => g.Nome.Replace("Grupo ", ""), g => g.Id);

        // 48 seleções, vinculadas ao grupo e com bandeira por código (ISO/FIFA).
        var selecoes = DadosCopa.Selecoes
            .Select(s => new Selecao
            {
                Nome = s.Nome,
                Codigo = s.Codigo,
                GrupoId = grupoPorLetra[s.Grupo],
                BandeiraUrl = DadosCopa.BandeiraUrl(s.Codigo),
            })
            .ToList();
        db.Selecoes.AddRange(selecoes);
        await db.SaveChangesAsync();

        // Índice por nome canônico (chave de junção entre os arquivos de /fontes).
        var selecaoPorNome = selecoes.ToDictionary(s => s.Nome, s => s.Id);

        // Ranking FIFA — apenas as seleções presentes na fonte (cobertura parcial).
        var rankings = DadosCopa.Ranking
            .Where(r => selecaoPorNome.ContainsKey(r.Selecao))
            .Select(r => new RankingFifa
            {
                SelecaoId = selecaoPorNome[r.Selecao],
                Posicao = r.Posicao,
                Pontuacao = r.Pontos,
            })
            .ToList();
        db.RankingFifa.AddRange(rankings);

        // 72 jogos da fase de grupos, todos com data, horário, estádio e cidade.
        var jogos = DadosCopa.Jogos
            .Select(j => new Jogo
            {
                Data = DateTime.SpecifyKind(DateTime.Parse(j.Data).Add(ParseHora(j.Hora)), DateTimeKind.Utc),
                SelecaoMandanteId = selecaoPorNome[j.Mandante],
                SelecaoVisitanteId = selecaoPorNome[j.Visitante],
                GrupoId = grupoPorLetra[j.Grupo],
                Estadio = j.Estadio,
                Cidade = j.Cidade,
            })
            .ToList();
        db.Jogos.AddRange(jogos);

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Popula o técnico de cada seleção e os jogadores convocados a partir dos
    /// arquivos oficiais de /fontes, vinculando por código FIFA (<see cref="Selecao.Codigo"/>).
    /// Idempotente: técnicos só são preenchidos quando ausentes; jogadores só são
    /// inseridos quando a tabela está vazia.
    /// </summary>
    private static async Task SeedElencoETecnicosAsync(AppDbContext db)
    {
        var selecoes = await db.Selecoes.ToListAsync();
        if (selecoes.Count == 0) return;

        var selecaoPorCodigo = selecoes
            .GroupBy(s => s.Codigo, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        // ----- Técnicos: preenche apenas seleções ainda sem técnico. -----
        if (selecoes.Any(s => s.Tecnico is null))
        {
            var caminhoTecnicos = CaminhoFonte("copa2026_pais_tecnicos.txt");
            if (File.Exists(caminhoTecnicos))
            {
                var tecnicos = ElencoParser.ParseTecnicos(caminhoTecnicos);
                foreach (var selecao in selecoes.Where(s => s.Tecnico is null))
                {
                    if (tecnicos.TryGetValue(selecao.Codigo, out var tecnico))
                    {
                        selecao.Tecnico = tecnico;
                    }
                    else
                    {
                        Console.WriteLine($"[Seed] Técnico não encontrado para o código '{selecao.Codigo}' ({selecao.Nome}).");
                    }
                }
                await db.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine($"[Seed] Arquivo de técnicos não encontrado: {caminhoTecnicos}");
            }
        }

        // ----- Jogadores: só popula quando a tabela está vazia. -----
        if (!await db.Jogadores.AnyAsync())
        {
            var caminhoJogadores = CaminhoFonte("copa2026_selecoes_jogadores.txt");
            if (File.Exists(caminhoJogadores))
            {
                var elencos = ElencoParser.ParseJogadores(caminhoJogadores);
                foreach (var (codigo, jogadoresSeed) in elencos)
                {
                    if (!selecaoPorCodigo.TryGetValue(codigo, out var selecao))
                    {
                        Console.WriteLine($"[Seed] Bloco de jogadores sem seleção correspondente para o código '{codigo}'.");
                        continue;
                    }

                    db.Jogadores.AddRange(jogadoresSeed.Select(j => new Jogador
                    {
                        Nome = j.Nome,
                        Idade = j.Idade,
                        Posicao = j.Posicao,
                        GolsMarcados = j.Gols,
                        ParticipacoesCopas = 0, // Sem dado na fonte.
                        SelecaoId = selecao.Id,
                    }));
                }
                await db.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine($"[Seed] Arquivo de jogadores não encontrado: {caminhoJogadores}");
            }
        }
    }

    /// <summary>Caminho de um arquivo de /fontes copiado para a saída (pasta <c>fontes</c>).</summary>
    private static string CaminhoFonte(string arquivo) =>
        Path.Combine(AppContext.BaseDirectory, "fontes", arquivo);

    /// <summary>
    /// Converte o horário oficial no formato "HHhMM" (ex.: "16h00", "20h30") em
    /// um <see cref="TimeSpan"/>, conforme transcrito de /fontes.
    /// </summary>
    private static TimeSpan ParseHora(string hora)
    {
        var partes = hora.Split('h', StringSplitOptions.RemoveEmptyEntries);
        var horas = int.Parse(partes[0]);
        var minutos = partes.Length > 1 ? int.Parse(partes[1]) : 0;
        return new TimeSpan(horas, minutos, 0);
    }
}

## Context

A Landing Page do PortalCopa26 já estabeleceu os padrões do projeto: componentes Blazor em `Components/Pages/<Capacidade>/`, acesso a dados via serviços que retornam DTOs (sem EF Core nos `.razor`), uso de `IDbContextFactory<AppDbContext>` com consultas `AsNoTracking`, e recursos visuais (bandeiras) servidos localmente. Os dados oficiais já estão semeados em `Jogos`, `Grupos` e `Selecoes` (`DbInitializer`/`DadosCopa`).

Esta change adiciona a página de Jogos seguindo esses mesmos padrões. O modelo `Jogo` já contém `Data`, `SelecaoMandante`, `SelecaoVisitante`, `Grupo`, `Estadio` e `Cidade` — não são necessárias alterações no modelo nem migrações.

Restrição relevante dos dados: o seed (`DadosCopa.Jogos` + `DbInitializer`) carrega a data via `DateTime.Parse("yyyy-MM-dd")`, ou seja, **sem horário** (sempre meia-noite). Os grupos são persistidos com `Grupo.Nome = "Grupo A" … "Grupo L"`.

## Goals / Non-Goals

**Goals:**
- Página `Jogos` com rota própria, composta por `Jogos`, `JogosFiltro`, `JogosDataHeader` e `JogoCard`.
- Listagem agrupada por data, ordenada por data e hora, com filtro por grupo.
- Exibir grupo e estádio (estádio + cidade) por partida.
- Reutilizar o padrão serviço + DTO (`JogosService`) já existente.
- Botão "Ver Grupos" como CTA.

**Non-Goals:**
- Jogos do mata-mata (não semeados) — somente a fase de grupos.
- Página de Grupos em si (o botão apenas aponta/sinaliza disponibilidade).
- Resultados oficiais/placar e qualquer simulação.
- Paginação ou busca textual.

## Decisions

**Decisão: `JogosService` + DTO `JogoListaDto`, espelhando `LandingPageService`.**
O serviço expõe `Task<IReadOnlyList<JogoListaDto>> ObterJogosAsync(int? grupoId = null)` e `Task<IReadOnlyList<GrupoOpcaoDto>> ObterGruposAsync()`. Reutiliza o `JogoResumoDto` conceitualmente, mas um DTO próprio mantém a página independente da Landing Page. Alternativa considerada: reaproveitar `JogoResumoDto` — descartada para evitar acoplamento entre capacidades distintas (CLAUDE.md: serviços/specs próprios por capacidade).

**Decisão: ordenação por `Data` com desempate estável por `Id`; agrupamento por grupo na UI.**
Como os dados não têm horário, ordenar "por hora" não desempata nada. O serviço ordena por `Data` e, em seguida, por `Id` (`OrderBy(j => j.Data).ThenBy(j => j.Id)`), garantindo ordem determinística e estável. Seguindo o protótipo (`../prototipo/jogos.html`), o componente `Jogos` agrupa a lista por `GrupoNome` com `GroupBy(...).OrderBy(g => g.Key)` (grupos A→L) e renderiza um cabeçalho `JogosGrupoHeader` ("Grupo X · Fase de grupos · N JOGOS") por grupo; dentro de cada grupo as partidas mantêm a ordem por data vinda do serviço. Alternativa considerada: agrupar por data (calendário cronológico) — descartada para reproduzir o visual do protótipo. Alternativa: agrupar no serviço retornando estrutura aninhada — descartada por adicionar complexidade ao DTO sem ganho real.

**Decisão: rota `/jogos` com render mode `InteractiveServer`.**
A página declara `@page "/jogos"` e `@rendermode InteractiveServer` (mesmo modo da `Home`). O render mode interativo é necessário porque o filtro depende de eventos de UI (`EventCallback` → recarregar lista); sob SSR estático o filtro não funcionaria.

**Decisão: rótulo do grupo a partir de `Grupo.Nome`.**
`GrupoOpcaoDto(Id, Nome)` usa diretamente `Grupo.Nome` ("Grupo A" … "Grupo L"), sem derivar ou transformar letras. O filtro emite o `Id` do grupo, não o texto.

**Decisão: filtro por `GrupoId` no serviço (consulta filtrada), não em memória.**
`ObterJogosAsync(grupoId)` aplica `Where(j => j.GrupoId == grupoId)` quando informado. Mantém a consulta eficiente e a lógica de dados na camada de serviço. O `JogosFiltro` emite um `EventCallback<int?>` para o componente pai `Jogos`, que recarrega a lista — fluxo unidirecional padrão do Blazor.

**Decisão: opções de grupo vindas do banco.**
`ObterGruposAsync()` projeta `Grupos` ordenados por `Nome` em `GrupoOpcaoDto(Id, Nome)`, garantindo que o filtro só ofereça grupos oficiais (A–L).

**Decisão: botão "Ver Grupos" em estado "em breve".**
A página de Grupos está fora do escopo desta change e não há âncora de grupos na aplicação. Para não criar link quebrado, o botão é exibido desabilitado/"em breve", sem disparar navegação. Quando a capacidade de grupos for implementada, basta apontá-lo para a rota correspondente.

## Risks / Trade-offs

- **Ausência de horário nos dados** → A ordem intra-dia poderia ficar indeterminada; mitigado com desempate por `Id` (`ThenBy`). A UI não exibe horário, então não há informação enganosa.
- **Datas semeadas no passado/futuro relativas a 2026-06-21 podem afetar a percepção de "próximos"** → A página de Jogos lista todos os jogos da fase de grupos (não apenas futuros), então não depende da data atual; ordenação cronológica é suficiente.
- **Acoplamento acidental com DTOs da Landing Page** → Mitigado com DTOs próprios em `Services/Dtos/JogosDtos.cs`.
- **Reconsulta a cada troca de filtro** → Volume pequeno (72 jogos); custo desprezível e mantém a lógica no serviço. Caso necessário no futuro, cachear no componente.

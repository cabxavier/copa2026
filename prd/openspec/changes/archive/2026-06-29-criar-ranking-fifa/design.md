## Context

O domínio de ranking já tem base de dados pronta: a entidade `Models/RankingFifa.cs` (Posição, Pontuação, `SelecaoId`/`Selecao`), a configuração EF `Data/Configurations/RankingFifaConfiguration.cs`, o `DbSet<RankingFifa>` em `AppDbContext` e o seed `Data/Seed/DadosCopa.cs` (`Ranking`, 18 registros) já consomem exclusivamente `fontes/copa2026_ranking_fifa.txt`. O seed propositalmente exclui Itália (12º) e Dinamarca (20º) por não disputarem a Copa.

Hoje o único consumo do ranking é `LandingPageService.ObterRankingTopAsync`, que retorna `RankingItemDto(Posicao, Selecao, Codigo, Pontuacao)` — **sem grupo** — e alimenta o gráfico de barras do Top 10 na Landing Page (`Components/Pages/LandingPage/RankingFifaChart.razor`).

Falta a página dedicada de Ranking. O protótipo de referência é `prototipo/ranking.html` (autocontido; tema escuro, pódio do Top 3 com medalhas ouro/prata/bronze, grupo, pesquisa e tabela; estilos do pódio/tabela estão inline no `<style>`). Ele exibe as **18 seleções reais** do arquivo oficial com pontuações fiéis e grupos corretos de `copa2026_grupos.txt`; apenas a largura da barra de pontuação (`ptsbar`) é estética. A página Blazor usa a pontuação real do seed.

O padrão do projeto (ver `Jogos`, `Selecoes`, `Simulador`): página + componentes em `Components/Pages/<Capacidade>/`, serviço + interface em `Services/`, DTOs em `Services/Dtos/`, `IDbContextFactory` para acesso assíncrono e `AsNoTracking`, registro em `Program.cs`. O menu efetivo do app está em `Components/Layout/MainLayout.razor` (`<nav class="menu">`), com estado ativo por rota (`IsJogos`/`IsGrupos`/…); `NavMenu.razor` é template default não utilizado. Hoje o link "Ranking" do `MainLayout` (menu e rodapé) aponta para a âncora `/#ranking` (gráfico da Landing Page).

## Goals / Non-Goals

**Goals:**
- Página `/ranking` fiel ao protótipo (tema escuro, fontes claras, destaque do Top 3).
- Exibir posição, bandeira FIFA, nome, pontuação FIFA e grupo.
- Pesquisa de seleções por nome em tempo real (client-side).
- Componentes Razor reutilizáveis: Top 3, tabela, linha e pesquisa.
- Serviço dedicado `RankingService`/`IRankingService` que entrega o grupo junto.
- Link "Ranking" no menu.

**Non-Goals:**
- Não alterar a entidade `RankingFifa`, a configuração EF nem o seed (já corretos e fiéis à fonte).
- Não incluir seleções fora da Copa nem inventar dados.
- Não refatorar o gráfico do Top 10 da Landing Page (`RankingFifaChart` permanece).
- Sem chamadas a APIs externas em runtime; as bandeiras (origem FIFA) são assets locais em `wwwroot/img/flags/{codigo}.png`, já preenchidas pelo seed em `Selecao.BandeiraUrl`.
- Sem persistência nova; ranking é leitura.

## Decisions

### 1. Serviço dedicado `RankingService` em vez de reutilizar `LandingPageService`
`LandingPageService.ObterRankingTopAsync` não traz o grupo e está acoplado à Landing Page. O CLAUDE.md prevê `RankingService` próprio. Criaremos `IRankingService.ObterRankingAsync()` retornando todos os itens ordenados por posição, cada um com o grupo.
- **Alternativa considerada**: estender `LandingPageService`. Rejeitada por violar a separação por capacidade e misturar responsabilidades.

### 2. Novo DTO `RankingFifaItemDto` com grupo
Novo record em `Services/Dtos/` (ex.: `RankingDtos.cs`): `RankingFifaItemDto(int Posicao, string Selecao, string Codigo, string? BandeiraUrl, decimal Pontuacao, string Grupo)`. O `RankingItemDto` existente da Landing Page permanece inalterado para não impactar o gráfico.
- **Grupo** obtido via `r.Selecao.Grupo.Nome` (a entidade `Selecao` tem `GrupoId`/`Grupo`). A query usa `Include(r => r.Selecao).ThenInclude(s => s.Grupo)`.

### 3. Pesquisa client-side
A lista do ranking é pequena (18 seleções). O serviço carrega tudo uma vez; o filtro por nome ocorre na página/componente de pesquisa via `@bind`/evento `oninput`, sem nova ida ao banco.
- **Alternativa**: filtro no serviço/DB. Rejeitada — desnecessário para o volume e prejudica a responsividade da digitação.

### 4. Decomposição em componentes
- `Ranking.razor` — página (`@page "/ranking"`), injeta `IRankingService`, carrega os dados, mantém o estado do termo de pesquisa e compõe os demais.
- `RankingPesquisa.razor` — campo de busca; `[Parameter] Termo` + `EventCallback<string> TermoChanged`.
- `RankingTop3.razor` — pódio das 3 primeiras posições (cards ouro/prata/bronze) com bandeira, nome, grupo e pontuação.
- `RankingTabela.razor` — recebe a lista filtrada e renderiza o cabeçalho e as linhas; trata o estado "nenhum resultado".
- `RankingLinha.razor` — uma linha (posição, bandeira, nome, pontuação, grupo); recebe um `RankingFifaItemDto` e um indicador de destaque.
- **Rationale**: reuso e legibilidade, alinhado ao padrão de `Jogos`/`Simulador`.

### 5. Estilo (tema escuro) — reutilizar `portal.css` global
O tema escuro **já é global** no app: `wwwroot/css/portal.css` está linkado em `App.razor`, define o `:root`/`body` e é reutilizado por todas as páginas (Jogos/Grupos/Seleções/Simulador) via classes globais (`.page`, `.panel`, `table`, `.tname`, `.rank-pos`, `.pts`, `.fl-img`, `.search`). A página de Ranking segue a mesma convenção: reutiliza essas classes e adiciona os estilos específicos (pódio do Top 3 e tabela de ranking, hoje inline em `ranking.html`) numa nova seção "Página Ranking" em `portal.css`, espelhando a seção "Página Grupos". As medalhas do pódio usam três tons — ouro (`#f5a623`, 1º), prata (`#c7d2e0`, 2º) e bronze (`#cd7f4f`, 3º) — conforme o protótipo; o ouro/prata reutilizam a paleta já adotada em `RankingFifaChart.razor`.
- **Alternativa considerada**: CSS isolado (`*.razor.css`). Rejeitada — não há tema a "vazar" (o app já é escuro globalmente); isolar contrariaria a convenção real do projeto e duplicaria tokens de cor, violando "evitar duplicação" do CLAUDE.md.

### 6. Bandeira: assets locais e fallback por erro de carregamento
As bandeiras vêm de `Selecao.BandeiraUrl` (`img/flags/{codigo}.png`), **sempre preenchida** pelo seed (`DbInitializer` linha 58). O risco real não é URL nula e sim o **PNG ausente / erro de carregamento (404)**. O fallback é tratado no `<img>` (ex.: `onerror` ocultando a imagem ou exibindo o código), não por checagem de nulidade.

### 7. Formato da pontuação
A pontuação é exibida com **duas casas decimais em cultura pt-BR** (ex.: `1877,72`), preservando exatamente o valor da fonte (`decimal`). Opcionalmente, a linha pode exibir a barra de progresso `ptsbar` do protótipo — elemento **puramente estético** (largura por fórmula relativa), sem vínculo com dado oficial.

### 8. Comportamento do Top 3 durante a pesquisa
O componente `RankingTop3` (pódio) é exibido **apenas quando não há termo de pesquisa**. Com termo ativo, somente a `RankingTabela` filtrada é mostrada. Sem pesquisa, o pódio aparece acima da tabela e as três primeiras seleções também constam nas primeiras linhas da tabela (com destaque de linha).
- **Alternativa considerada**: manter o pódio fixo durante a busca. Rejeitada por confundir o usuário ao pesquisar uma seleção fora do Top 3.

## Risks / Trade-offs

- **Cobertura parcial do ranking (18 de 20 do arquivo)** → Comportamento correto e proposital (seleções fora da Copa não têm grupo a exibir). Documentado no seed; a spec exige apenas seleções da Copa.
- **Divergência com a Landing Page** (o gráfico do Top 10 em barras de `RankingFifaChart` permanece) → A página de Ranking é a visão completa (pódio, grupo, pesquisa, tabela) conforme `ranking.html`; o gráfico da Landing Page não é alterado. Mantém-se o tema escuro e o destaque ouro/prata/bronze do pódio; o painel promocional "Simule a Copa!" é omitido (ver Open Questions).
- **Bandeira ausente (PNG inexistente em `wwwroot/img/flags/`)** → `BandeiraUrl` nunca é nulo (sempre preenchida pelo seed); o risco é arquivo ausente/erro de carregamento. A linha degrada com elegância (sem imagem quebrada via `onerror`), exibindo apenas o nome/código.
- **Acoplamento ao grupo via navegação EF** → Garantir os `Include`/`ThenInclude` para evitar `NullReferenceException` ao projetar o grupo.

## Migration Plan

Mudança aditiva, sem migração de banco (entidade e seed já existem). Passos: criar DTO e serviço → registrar em `Program.cs` → criar componentes e página → adicionar link no menu → validar build/execução. Rollback: remover os arquivos novos e o registro do serviço; nada no schema é afetado.

## Open Questions

- **Painel promocional "Simule a Copa!"** (RESOLVIDO): será **omitido** nesta página. A página prioriza Top 3 + pesquisa + tabela completa; o painel promocional do protótipo não agrega aos requisitos do ranking e fica fora do escopo.

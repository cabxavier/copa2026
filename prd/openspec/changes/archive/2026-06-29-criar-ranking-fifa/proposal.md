## Why

O PortalCopa26 ainda não possui a página de **Ranking FIFA** prevista no escopo. Hoje o ranking só aparece de forma resumida (gráfico de barras do Top 10) na Landing Page, sem página dedicada, sem pesquisa e sem exibir o grupo de cada seleção. As entidades e o seed já existem, mas faltam a página, os componentes reutilizáveis e um serviço próprio que entregue posição, seleção, pontuação FIFA e grupo conforme o protótipo `prototipo/ranking.html`.

## What Changes

- Criar a página **Ranking** (`/ranking`) seguindo o layout e o tema escuro do protótipo `ranking.html`, com fontes em cor clara para contraste com o fundo escuro.
- Exibir, para cada seleção: **posição**, **bandeira** (asset local em `wwwroot/img/flags/{codigo}.png`, de origem FIFA, via `Selecao.BandeiraUrl`), **nome da seleção**, **pontuação FIFA** e **grupo**.
- Destacar visualmente as **três primeiras posições** num pódio com cores **ouro (1º), prata (2º) e bronze (3º)**, exibindo bandeira, nome, grupo e pontuação, como no protótipo.
- Permitir **pesquisar seleções** por nome, filtrando a tabela em tempo real.
- Decompor a interface em **componentes Razor reutilizáveis**: Top 3, tabela de ranking, linha da tabela e campo de pesquisa — em vez de concentrar tudo na página.
- Criar um **serviço dedicado** `RankingService` (`IRankingService`) que entrega os dados do ranking já com o grupo, sem acesso ao `DbContext` em páginas/componentes.
- Repontar o link **Ranking** do menu (em `MainLayout.razor`) da âncora `/#ranking` (gráfico da Landing Page) para a nova rota `/ranking`, com estado ativo próprio (`IsRanking`), incluindo o link do rodapé.
- Reutilizar a entidade `RankingFifa` e o `SeedData` existentes (`DadosCopa.Ranking`), que já consomem exclusivamente `fontes/copa2026_ranking_fifa.txt` e excluem seleções fora da Copa (Itália 12º, Dinamarca 20º). Nenhum dado novo é inventado.

## Capabilities

### New Capabilities
- `ranking`: Página e componentes que apresentam o ranking FIFA oficial das seleções da Copa 2026, com destaque do Top 3, exibição de grupo e pesquisa por seleção, alimentados por um serviço dedicado.

### Modified Capabilities
<!-- Nenhuma capability existente tem requisitos alterados. A entidade RankingFifa e o seed já existem (introduzidos em mudanças anteriores) e são apenas reutilizados. -->

## Impact

- **Nova capability/spec**: `openspec/specs/ranking`.
- **Novos componentes** (`Components/Pages/Ranking/`): `Ranking.razor` (página, rota `/ranking`), `RankingTop3.razor`, `RankingTabela.razor`, `RankingLinha.razor`, `RankingPesquisa.razor`.
- **Novo serviço**: `Services/IRankingService.cs`, `Services/RankingService.cs` e DTO de ranking (em `Services/Dtos/`) incluindo o grupo; registro em `Program.cs`.
- **Navegação**: `Components/Layout/MainLayout.razor` (menu real do app — repontar o link "Ranking" para `/ranking`, adicionar `IsRanking` e atualizar o link do rodapé). `NavMenu.razor` é template default não utilizado e não será alterado.
- **Reuso sem alteração de dados**: `Models/RankingFifa.cs`, `Data/Configurations/RankingFifaConfiguration.cs`, `Data/Seed/DadosCopa.cs` (`Ranking`), `AppDbContext.RankingFifa`.
- **Estilos**: reutilizar as classes globais já existentes em `wwwroot/css/portal.css` (`.page`, `.panel`, `table`, `.tname`, `.rank-pos`, `.pts`, `.fl-img`, `.search`) e acrescentar os estilos específicos do ranking (pódio do Top 3 e tabela) numa nova seção "Página Ranking" no próprio `portal.css`, espelhando a seção "Página Grupos". O tema escuro já é global no app; sem CSS isolado e sem frameworks além do Bootstrap 5.
- **Fonte de dados**: exclusivamente `fontes/copa2026_ranking_fifa.txt` (já refletida no seed). Sem chamadas a APIs externas em runtime — as bandeiras (origem FIFA) são servidas como assets locais em `wwwroot/img/flags/` via `Selecao.BandeiraUrl`.

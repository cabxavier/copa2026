## Why

A fundação do PortalCopa26 está pronta (Blazor Web App, EF Core, SQLite, entidades e SeedData), mas não há nenhuma página de produto. A Landing Page é a porta de entrada do portal e o primeiro valor entregue ao usuário: apresenta a Copa 2026, os próximos jogos, o ranking FIFA (com gráfico) e convida ao simulador. Implementá-la agora valida ponta a ponta a stack (serviços → EF Core → SQLite → componentes → Chart.js via JSInterop) e estabelece os padrões reutilizáveis (componentes, serviços de dados, wrapper de gráficos) para as demais páginas.

Além disso, a CLAUDE.md passou a exigir o uso **exclusivo** dos dados oficiais em `/fontes` (proibido dados fictícios). O SeedData atual usa um conjunto representativo/fictício; como a Landing Page exibe ranking, jogos e estatísticas reais, é necessário corrigir a carga de dados para as entidades que a página apresenta.

## What Changes

- Criar a página inicial composta por componentes reutilizáveis em `Components/Pages/LandingPage/`:
  - `HeroSection.razor` — apresentação da Copa 2026 e países-sede.
  - `EstatisticasCopa.razor` — números do torneio (seleções, grupos, estádios, jogos, cidades-sede).
  - `ProximosJogos.razor` — próximos jogos por data, com grupo e estádio.
  - `RankingFifaChart.razor` — gráfico de barras do Ranking FIFA (Top 10) via Chart.js + JSInterop.
  - `SimuladorPainel.razor` — chamada (call-to-action) para o simulador.
- A página `Home.razor` passa a **compor** esses componentes (sem concentrar lógica/markup).
- Criar **serviços de acesso a dados** da Landing Page (camada `Services/`) consumidos por injeção de dependência; **proibir** consultas EF Core diretamente nos componentes.
- Introduzir um **wrapper JSInterop reutilizável de Chart.js** (módulo JS + serviço) projetado para futuros gráficos estatísticos.
- Adicionar o Chart.js ao projeto (via `wwwroot`) e registrar o módulo JSInterop.
- **MODIFICAR** a carga de dados: substituir o SeedData fictício pelos dados reais de `/fontes` para as entidades que a Landing Page exibe — grupos (12), seleções (48), ranking FIFA (apenas as seleções presentes em `copa2026_ranking_fifa.txt`, cobertura parcial) e jogos da fase de grupos (72, com data/estádio/cidade). Os códigos de seleção e as bandeiras usam o código oficial de país (ISO/FIFA), pois `/fontes` não os fornece. Carga de jogadores e do mata-mata permanece fora deste escopo.
- A estatística "Jogos" exibe o total **oficial** do torneio (104, conforme `copa2026_fases.txt`) e a de "Estádios" exibe 16; os países-sede do Hero (EUA 11, México 3, Canadá 2) vêm de `copa2026_cidades_sede_estadios.txt`.

## Capabilities

### New Capabilities
- `landing-page`: a página inicial e seus componentes (hero, estatísticas, próximos jogos, ranking FIFA com gráfico, painel do simulador), os serviços de dados que a alimentam e o wrapper reutilizável de Chart.js via JSInterop.

### Modified Capabilities
- `persistencia-dados`: o requisito de carga de dados via SeedData passa a usar **exclusivamente** os dados oficiais de `/fontes` (sem dados fictícios) para grupos, seleções, ranking FIFA, sedes/estádios e jogos da fase de grupos.

## Impact

- **Código novo**: `Components/Pages/LandingPage/*.razor`, serviços em `Services/` (ex.: `LandingPageService`/serviços por seção), módulo JS `wwwroot/js/charts.js` e serviço de interop de gráficos.
- **Dependências**: Chart.js servido via `wwwroot` (sem APIs externas); Bootstrap 5 (já presente no template).
- **Dados**: reescrita do `DbInitializer`/SeedData para ler os arquivos de `/fontes`; o arquivo SQLite local precisará ser recriado para refletir os dados reais.
- **Configuração**: registro dos novos serviços e do interop em `Program.cs`; referência ao script Chart.js em `App.razor`.
- **Specs afetadas**: nova `landing-page`; modificação na `persistencia-dados` (carga de dados).
- **Fora do escopo**: páginas Jogos, Grupos, Seleções, Ranking completo e Simulador; navegação avançada; responsividade avançada; lógica do simulador; carga de jogadores e dos jogos do mata-mata.

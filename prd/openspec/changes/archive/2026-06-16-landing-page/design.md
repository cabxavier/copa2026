## Context

A fundação (solução Blazor Web App, EF Core + SQLite, `AppDbContext`, entidades e SeedData) já existe e está arquivada. A Landing Page é a primeira página de produto e deve ser composta por componentes reutilizáveis, alimentados por serviços (sem EF Core nos componentes), exibindo um gráfico de Ranking FIFA com Chart.js via JSInterop.

Restrições atuais relevantes:
- O protótipo existe em `..\prototipo` (`index.html`, `ranking.html`, `styles.css`, `data.js` etc.) e serve como **referência visual** (layout do hero com 3 países-sede, cards de estatísticas, cards de jogos, painel "Simule a Copa!", design system em `styles.css`). Porém, os **dados** do protótipo (`data.js`) são fictícios e divergem de `/fontes`, e o gráfico de ranking do protótipo é feito com barras CSS — ambos serão substituídos conforme as decisões abaixo.
- A CLAUDE.md passou a exigir uso **exclusivo** dos dados oficiais em `/fontes`. O SeedData atual é fictício, então esta mudança também corrige a carga de dados das entidades exibidas pela Landing Page.
- Render mode atual: Interactive Server; `AppDbContext` registrado via `IDbContextFactory`.

## Goals / Non-Goals

**Goals:**
- `Home.razor` compondo 5 componentes em `Components/Pages/LandingPage/`.
- Serviços de dados em `Services/` consumidos por DI; nenhum acesso EF Core nos `.razor`.
- Wrapper de Chart.js reutilizável (módulo JS + serviço de interop) para gráfico de barras do Ranking FIFA (Top 10) e gráficos futuros.
- SeedData reescrito a partir de `/fontes` para grupos, seleções, ranking, sedes/estádios e jogos da fase de grupos.

**Non-Goals:**
- Páginas Jogos, Grupos, Seleções, Ranking completo e Simulador.
- Lógica do simulador; navegação avançada; responsividade avançada.
- Carga de jogadores e dos jogos do mata-mata.
- Integração com APIs externas (Chart.js é servido localmente).

## Decisions

### Protótipo como referência visual; dados e gráfico conforme requisitos
O protótipo guia o **visual** (estrutura do hero, cards de estatísticas 48/12/104/16, cards de jogos, painel promocional do simulador, paleta/estilos de `styles.css`), mas três pontos divergentes são resolvidos a favor da CLAUDE.md e do escopo solicitado:
1. **Dados**: usar exclusivamente `/fontes` (não `data.js`, que é fictício e inconsistente — ex.: grupos B/D/H/K/L diferentes do oficial).
2. **Gráfico de ranking**: usar Chart.js via JSInterop (o protótipo usa barras CSS), estilizado para aproximar o visual do protótipo.
3. **Posição do ranking**: incluir o Ranking FIFA (Top 10) na própria Landing Page (no protótipo o ranking está em página separada), conforme a CLAUDE.md e o escopo desta mudança.

Rationale: o protótipo não é fonte de dados nem de tecnologia de gráfico; é referência de aparência. Requisitos normativos prevalecem.

### Composição de componentes
`Home.razor` apenas orquestra; cada seção é um componente próprio em `Components/Pages/LandingPage/`. Componentes recebem dados de serviços via `@inject` e carregam em `OnInitializedAsync` (o `RankingFifaChart` cria o gráfico em `OnAfterRenderAsync`). Rationale: legibilidade, reuso e aderência ao pedido explícito de não concentrar tudo em uma página.

### Camada de serviços e DTOs
Criar serviços de leitura (ex.: `ILandingPageService` com métodos `ObterEstatisticasAsync`, `ObterProximosJogosAsync`, `ObterRankingTopAsync`, `ObterPaisesSedeAsync`) que usam `IDbContextFactory<AppDbContext>` e retornam **DTOs/ViewModels** (não entidades EF). Rationale: isola a UI do EF Core (requisito), evita problemas de tracking/lazy-loading em Blazor Server e prepara extração futura para camadas. Alternativa considerada: injetar `AppDbContext` nos componentes — descartada por violar o requisito e por risco de concorrência.

### Integração Chart.js via JSInterop (reutilizável)
Servir `chart.js` em `wwwroot/lib/chartjs/` e um módulo ES `wwwroot/js/charts.js` com funções `renderBarChart(canvasId, config)` e `destroyChart(canvasId)`. Em .NET, um serviço `ChartInterop` (scoped) carrega o módulo via `IJSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/charts.js")` e expõe métodos tipados. O `RankingFifaChart` chama o interop em `OnAfterRenderAsync(firstRender)` (DOM disponível) e implementa `IAsyncDisposable` para destruir o gráfico. Rationale: JSInterop só funciona após render no Server; módulo isolado permite reuso por qualquer gráfico futuro passando rótulos/valores. Alternativa: script global inline — descartada por não ser reutilizável nem encapsulado.

### Origem do Chart.js (local vs CDN)
Servir localmente em `wwwroot` (sem CDN). Rationale: a CLAUDE.md veta integração com APIs externas e favorece funcionamento offline/local.

### Reescrita do SeedData a partir de /fontes
O `DbInitializer` passa a montar os dados a partir dos arquivos de `/fontes`. Abordagem: dados oficiais transcritos para estruturas de seed em C# derivadas fielmente dos arquivos (parsing em tempo de build não é necessário, pois os arquivos são estáticos e pequenos), garantindo nomes de seleções, composição dos 12 grupos, 72 confrontos da fase de grupos, 16 estádios/sedes e ranking FIFA reais. O arquivo `copa2026_jogos_primeira_fase.txt` fornece **todos os 72 jogos com data, estádio e cidade completos** — nenhuma data ou estádio precisa ser inferido. Rationale: cumpre a proibição de dados fictícios; mantém idempotência (verifica existência antes de inserir). Trade-off: o arquivo SQLite existente precisa ser recriado para refletir os dados reais.

### Códigos ISO e bandeiras das seleções
`/fontes` não fornece os códigos de 3 letras das seleções (apenas nomes). Decisão: manter um mapa estático `nome canônico → código` (ISO 3166-1 alpha-3 / código FIFA) no seed, usado para `Selecao.Codigo` e para compor a `BandeiraUrl` no padrão FIFA (`https://api.fifa.com/api/v3/picture/flags-sq-4/{codigo}`). Rationale: códigos de país são identificadores factuais e padronizados — não são "dados de torneio inventados"; sem eles a entidade `Selecao` (campo obrigatório) e as bandeiras não podem ser preenchidas. Alternativa considerada: extrair de `data.js` — descartada (fictício e com seleções divergentes). O vínculo entre arquivos é feito por **nome canônico** de `copa2026_grupos.txt`.

### Países-sede e contagem de estádios (sem nova entidade)
O Hero e a estatística de estádios usam dados de `copa2026_cidades_sede_estadios.txt` (16 estádios: 11 EUA, 3 México, 2 Canadá). Decisão: **não** introduzir entidade `Estadio` nesta change; expor os países-sede (com contagem de estádios) e o total de estádios via constantes derivadas fielmente desse arquivo, retornadas pelo serviço como DTOs. Rationale: mantém o escopo enxuto e evita migração adicional; os dados são pequenos e estáveis. Trade-off: contagem por país não é consultável no banco (aceitável para a Landing Page). Uma entidade `Estadio` poderá ser introduzida quando a página de Jogos/Estádios exigir consulta.

### Semântica da estatística "Jogos" (104, não 72)
A estatística exibe os **números oficiais do torneio**: 48 seleções, 12 grupos, 16 estádios e **104 jogos**. Seleções e grupos coincidem com contagens do banco; estádios (16) e o total de jogos (104, incluindo o mata-mata ainda não semeado, conforme `copa2026_fases.txt`) provêm de `/fontes`. Decisão: **não** contar os 72 jogos semeados para esse card, evitando exibir um total parcial. Rationale: alinha com o protótipo e com a CLAUDE.md; o card representa o torneio, não o subconjunto carregado.

### Próximos jogos com data de referência
"Próximos jogos" filtra por `Data >= hoje` ordenado crescente; se não houver futuros (base é jun/2026 e a data atual do ambiente é 15/06/2026), exibe os primeiros jogos do torneio por data. Rationale: a Landing Page sempre mostra conteúdo útil.

## Risks / Trade-offs

- **JSInterop antes do render no Blazor Server** → Mitigação: inicializar o gráfico em `OnAfterRenderAsync(firstRender)`, nunca em `OnInitializedAsync`.
- **Vazamento de instâncias Chart.js** → Mitigação: `destroyChart` no `DisposeAsync` do componente.
- **Recriação do banco com dados reais** pode confundir quem tem o `.db` antigo → Mitigação: documentar que o `portalcopa26.db` deve ser apagado/recriado; SeedData idempotente repopula.
- **Transcrição manual de /fontes pode introduzir erros** → Mitigação: derivar estritamente dos arquivos (jogos com data/estádio completos no arquivo de origem), cobrir com cenário de spec "uso exclusivo dos dados oficiais"; conferir contagens (12/48/72/16).
- **Junção por nome entre arquivos** (ranking ↔ seleção ↔ jogo) é sensível a variações/acentos → Mitigação: adotar os nomes canônicos de `copa2026_grupos.txt` como chave e normalizar comparações; ranking só para nomes presentes na fonte.
- **Ranking parcial (20 de 48)** pode surpreender em telas futuras → Mitigação: `RankingFifa` é opcional por seleção; Top 10 não é afetado (as 10 primeiras estão nos grupos).
- **Protótipo como referência visual** pode divergir do resultado em Blazor → Mitigação: espelhar `index.html`/`ranking.html` e `styles.css`, ajustando ao Bootstrap 5.

## Migration Plan

1. Adicionar Chart.js em `wwwroot/lib/chartjs/` e o módulo `wwwroot/js/charts.js`; referenciar em `App.razor`.
2. Criar DTOs e serviços em `Services/`; registrar em `Program.cs`.
3. Reescrever `DbInitializer`/SeedData a partir de `/fontes`; apagar o `portalcopa26.db` antigo para repopular.
4. Criar os 5 componentes em `Components/Pages/LandingPage/` e compor em `Home.razor`.
5. Implementar `ChartInterop` e o `RankingFifaChart`.
6. Validar build, execução, dados reais no banco e renderização do gráfico.
- Rollback: remover os componentes/serviços novos e restaurar o SeedData anterior; sem dados de produção envolvidos.

## Open Questions

- Nenhuma pendente. O protótipo está disponível como referência visual e as divergências (dados, gráfico, posição do ranking) já têm decisão registrada.

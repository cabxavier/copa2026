## 1. Dados reais (SeedData a partir de /fontes)

- [x] 1.1 Transcrever de `/fontes` os 12 grupos e as 48 seleções (nomes canônicos de `copa2026_grupos.txt` e composição dos grupos A–L)
- [x] 1.2 Definir o mapa estático `nome canônico → código` (ISO 3166-1 alpha-3 / FIFA) das 48 seleções e compor `BandeiraUrl` no padrão FIFA `flags-sq-4/{codigo}`
- [x] 1.3 Transcrever o ranking FIFA (`copa2026_ranking_fifa.txt`) — apenas as seleções presentes no arquivo (cobertura parcial), sem inventar valores
- [x] 1.4 Transcrever os 72 jogos da fase de grupos (`copa2026_jogos_primeira_fase.txt`), todos com data, estádio e cidade; vincular seleções por nome canônico
- [x] 1.5 Reescrever o `DbInitializer`/SeedData para inserir esses dados de forma idempotente (sem dados fictícios)
- [x] 1.6 Recriar o `portalcopa26.db` e validar contagens reais (12 grupos, 48 seleções, 72 jogos, 16 estádios distintos; ranking apenas para as seleções da fonte)

## 2. Serviços de dados (Services/)

- [x] 2.1 Criar DTOs/ViewModels da Landing Page (estatísticas, jogo resumido, item de ranking, país-sede)
- [x] 2.2 Criar `ILandingPageService` + implementação usando `IDbContextFactory<AppDbContext>` (métodos async, retornando DTOs)
- [x] 2.3 Implementar `ObterEstatisticasAsync` (48/12/16/104: seleções e grupos do banco; estádios=16 e jogos=104 de `/fontes`), `ObterProximosJogosAsync`, `ObterRankingTopAsync(int n)`
- [x] 2.4 Implementar `ObterPaisesSedeAsync` a partir de constantes derivadas de `copa2026_cidades_sede_estadios.txt` (EUA 11, México 3, Canadá 2)
- [x] 2.5 Registrar o(s) serviço(s) na injeção de dependência em `Program.cs`

## 3. Infraestrutura Chart.js + JSInterop reutilizável

- [x] 3.1 Adicionar o Chart.js em `wwwroot/lib/chartjs/` (servido localmente, sem CDN)
- [x] 3.2 Criar o módulo `wwwroot/js/charts.js` com `renderBarChart(canvasId, config)` e `destroyChart(canvasId)`
- [x] 3.3 Criar o serviço `ChartInterop` (.NET) que importa o módulo via `IJSObjectReference` e expõe métodos tipados
- [x] 3.4 Referenciar o necessário em `App.razor` e registrar o `ChartInterop` na DI

## 4. Componentes da Landing Page (Components/Pages/LandingPage/)

- [x] 4.1 `HeroSection.razor` — identidade da Copa 2026 e países-sede com contagem de estádios (11/3/2)
- [x] 4.2 `EstatisticasCopa.razor` — números oficiais do torneio (48/12/16/104) via serviço
- [x] 4.3 `ProximosJogos.razor` — lista ordenada por data (seleções, grupo, estádio) via serviço
- [x] 4.4 `RankingFifaChart.razor` — gráfico de barras Top 10 usando `ChartInterop` em `OnAfterRenderAsync` e `IAsyncDisposable`
- [x] 4.5 `SimuladorPainel.razor` — call-to-action para o simulador

## 5. Composição da página inicial

- [x] 5.1 Refatorar `Home.razor` para compor os 5 componentes (sem markup/lógica concentrados)
- [x] 5.2 Aplicar layout com Bootstrap 5 espelhando o visual do protótipo (`..\prototipo`: hero com países-sede, cards de estatísticas, cards de jogos, painel do simulador), sem usar os dados fictícios de `data.js`

## 7. Refinamentos visuais (protótipo + assets locais)

- [x] 7.1 Adicionar o logo oficial da FIFA no hero, na marca do cabeçalho e no badge
- [x] 7.2 Adicionar as ações do hero ("Abrir Simulador", "Ver Jogos") com âncoras para as seções
- [x] 7.3 Exibir países-sede na ordem EUA/Canadá/México com bandeiras (imagem)
- [x] 7.4 Aplicar a paleta de cores do pódio no gráfico (ouro/prata/azul do protótipo)
- [x] 7.5 Converter os links de menu/rodapé em âncoras das seções (`#top`, `#proximos-jogos`, `#ranking`, `#simulador`); Grupos/Seleções como "em breve"
- [x] 7.6 Baixar logo e bandeiras para `wwwroot/img` e apontar `DadosCopa` para os caminhos locais (sem dependência de rede)
- [x] 7.7 Recriar o `portalcopa26.db` para persistir as bandeiras locais em `Selecao.BandeiraUrl`

## 6. Verificação

- [x] 6.1 `dotnet build` sem erros/avisos relevantes
- [x] 6.2 Executar a aplicação e abrir a Landing Page; confirmar que todas as 5 seções renderizam
- [x] 6.3 Confirmar que o gráfico de Ranking FIFA (Top 10) renderiza com dados reais e é destruído ao sair
- [x] 6.4 Confirmar que próximos jogos refletem os dados reais e que a estatística exibe 48/12/16/104
- [x] 6.5 Confirmar que as bandeiras carregam (código FIFA) e que os países-sede mostram 11/3/2 estádios
- [x] 6.6 Verificar que nenhum componente `.razor` usa `AppDbContext`/EF Core diretamente

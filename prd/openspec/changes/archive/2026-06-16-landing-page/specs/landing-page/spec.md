## ADDED Requirements

### Requirement: Página inicial composta por componentes
A página inicial (`Home`) SHALL ser composta por componentes Blazor reutilizáveis, sem concentrar markup e lógica em um único arquivo. Os componentes SHALL residir em `Components/Pages/LandingPage/` e incluir `HeroSection`, `EstatisticasCopa`, `ProximosJogos`, `RankingFifaChart` e `SimuladorPainel`.

#### Scenario: Home compõe os componentes
- **WHEN** a página inicial é renderizada
- **THEN** ela exibe as seções hero, estatísticas, próximos jogos, ranking FIFA e o painel do simulador, cada uma renderizada por seu respectivo componente

#### Scenario: Componentes isolados em arquivos próprios
- **WHEN** o código da Landing Page é inspecionado
- **THEN** cada seção está em seu próprio arquivo `.razor` em `Components/Pages/LandingPage/`, e não embutida em `Home.razor`

### Requirement: Hero Section com países-sede
O componente `HeroSection` SHALL apresentar a Copa do Mundo 2026 e os três países-sede na ordem Estados Unidos, Canadá e México, cada um com sua bandeira (imagem) e a quantidade de estádios conforme `/fontes` (Estados Unidos 11, Canadá 2, México 3). O hero SHALL exibir o logo oficial da Copa do Mundo FIFA 2026 e oferecer chamadas de ação para o simulador e para os jogos.

#### Scenario: Exibição do hero
- **WHEN** a Landing Page é carregada
- **THEN** o `HeroSection` exibe o logo da Copa 2026, o título/identidade e os três países-sede (Estados Unidos, Canadá, México) com bandeira e quantidade de estádios

#### Scenario: Ações do hero
- **WHEN** o usuário aciona "Abrir Simulador" ou "Ver Jogos" no hero
- **THEN** a página rola, via âncora, para a seção do simulador ou de próximos jogos, respectivamente

### Requirement: Estatísticas da Copa
O componente `EstatisticasCopa` SHALL exibir os números oficiais do torneio conforme `/fontes`: 48 seleções, 12 grupos, 16 estádios e 104 jogos. As contagens de seleções e grupos refletem os dados persistidos; o total de estádios e o total de jogos (104, incluindo o mata-mata não semeado) provêm dos dados oficiais de `/fontes`, obtidos por meio de serviço.

#### Scenario: Exibição das estatísticas
- **WHEN** a Landing Page é carregada
- **THEN** o `EstatisticasCopa` exibe 48 seleções, 12 grupos, 16 estádios e 104 jogos, obtidos via serviço de dados

#### Scenario: Total de jogos é o oficial do torneio
- **WHEN** o número de jogos é exibido
- **THEN** o valor é 104 (total do torneio segundo `/fontes`/`copa2026_fases.txt`), e não apenas os 72 jogos da fase de grupos semeados

### Requirement: Próximos jogos
O componente `ProximosJogos` SHALL listar os próximos jogos ordenados por data, exibindo as seleções, o grupo e o estádio de cada partida.

#### Scenario: Listagem ordenada por data
- **WHEN** a Landing Page é carregada
- **THEN** o `ProximosJogos` exibe uma lista de jogos ordenada por data crescente, com seleções, grupo e estádio

#### Scenario: Apenas jogos futuros priorizados
- **WHEN** existem jogos com data futura em relação à data atual
- **THEN** o componente prioriza a exibição dos próximos jogos a ocorrer

### Requirement: Gráfico do Ranking FIFA via Chart.js
O componente `RankingFifaChart` SHALL exibir um gráfico de barras com as 10 seleções de maior pontuação no ranking FIFA, renderizado com Chart.js integrado via JSInterop.

#### Scenario: Renderização do Top 10
- **WHEN** a Landing Page é carregada
- **THEN** o `RankingFifaChart` renderiza um gráfico de barras com as 10 maiores pontuações do ranking FIFA, com nome da seleção e pontuação

#### Scenario: Dados originados do banco via serviço
- **WHEN** o gráfico é montado
- **THEN** os dados do ranking são obtidos de um serviço de dados (não de consulta EF Core direta no componente)

#### Scenario: Paleta de cores do pódio
- **WHEN** as barras do Top 10 são renderizadas
- **THEN** o 1º e o 3º colocados usam a cor ouro (#f5a623), o 2º usa prata (#c7d2e0) e os demais usam azul (#3b82f6), conforme o protótipo

### Requirement: Wrapper reutilizável de Chart.js
O sistema SHALL prover um wrapper de Chart.js via JSInterop (módulo JavaScript + abstração .NET) que permita renderizar gráficos a partir de dados fornecidos, reutilizável por futuras visualizações estatísticas do portal.

#### Scenario: Reutilização para novo gráfico
- **WHEN** uma nova visualização estatística precisa de um gráfico
- **THEN** ela pode reutilizar o wrapper passando rótulos e valores, sem reescrever a integração JSInterop

#### Scenario: Ciclo de vida do gráfico
- **WHEN** um componente que usa o wrapper é renderizado e posteriormente descartado
- **THEN** o gráfico é criado após a renderização (interop disponível) e os recursos JS são liberados ao descartar o componente

### Requirement: Chamada para o simulador
O componente `SimuladorPainel` SHALL exibir uma chamada (call-to-action) que direcione o usuário ao simulador.

#### Scenario: Exibição do call-to-action
- **WHEN** a Landing Page é carregada
- **THEN** o `SimuladorPainel` exibe um convite visível para acessar o simulador

### Requirement: Acesso a dados por serviços
Os componentes da Landing Page SHALL obter dados exclusivamente por meio de serviços injetados (camada `Services/`), e NÃO SHALL executar consultas EF Core diretamente.

#### Scenario: Componente consome serviço
- **WHEN** um componente da Landing Page precisa de dados
- **THEN** ele recebe um serviço via injeção de dependência e chama métodos assíncronos desse serviço

#### Scenario: Ausência de EF Core nos componentes
- **WHEN** o código dos componentes da Landing Page é inspecionado
- **THEN** não há uso direto de `AppDbContext`/consultas EF Core nos arquivos `.razor`

### Requirement: Navegação por âncoras na página
Os links de navegação da página (menu do cabeçalho e rodapé) e as chamadas de ação SHALL utilizar âncoras para seções da própria Landing Page, sem depender de páginas ainda não implementadas. As seções com âncora SHALL incluir topo, próximos jogos, ranking e simulador.

#### Scenario: Navegação interna por âncora
- **WHEN** o usuário aciona um link de Home, Jogos, Ranking ou Simulador (cabeçalho ou rodapé)
- **THEN** a página rola suavemente para a seção correspondente (`#top`, `#proximos-jogos`, `#ranking`, `#simulador`)

#### Scenario: Itens sem seção na Landing Page
- **WHEN** o item de menu corresponde a uma página fora do escopo desta change (Grupos, Seleções)
- **THEN** ele é exibido como indisponível ("em breve"), sem link quebrado

### Requirement: Recursos visuais servidos localmente
O logo da FIFA e as bandeiras das seleções SHALL ser servidos localmente a partir de `wwwroot` (baixados do padrão da API pública da FIFA), de modo que a Landing Page não dependa de acesso de rede em runtime para exibir essas imagens.

#### Scenario: Imagens carregam sem rede externa
- **WHEN** a Landing Page é exibida sem acesso a `api.fifa.com`
- **THEN** o logo e as bandeiras continuam sendo exibidos, pois são arquivos locais em `wwwroot`

## ADDED Requirements

### Requirement: Página de Ranking FIFA

O sistema SHALL disponibilizar uma página de Ranking FIFA acessível pela rota `/ranking`, apresentando as seleções da Copa 2026 ordenadas pela posição oficial do ranking FIFA, com tema escuro e fontes em cor clara para contraste com o fundo, seguindo o protótipo `prototipo/ranking.html`.

#### Scenario: Acessar a página de ranking
- **WHEN** o usuário navega para `/ranking`
- **THEN** a página exibe o título "Ranking" e a lista das seleções ordenadas crescentemente pela posição do ranking FIFA

#### Scenario: Contraste com fundo escuro
- **WHEN** a página de ranking é renderizada
- **THEN** o fundo é escuro e os textos (posição, nome, pontuação e grupo) são apresentados em cor clara, garantindo legibilidade conforme o protótipo

### Requirement: Exibição de dados do ranking

Para cada seleção do ranking o sistema SHALL exibir a posição, a bandeira da seleção (asset local em `wwwroot/img/flags/{codigo}.png`, via `Selecao.BandeiraUrl`), o nome da seleção, a pontuação FIFA e o grupo ao qual a seleção pertence.

#### Scenario: Conteúdo de cada linha
- **WHEN** uma linha do ranking é exibida
- **THEN** ela apresenta a posição, a bandeira da seleção, o nome da seleção, a pontuação FIFA formatada com duas casas decimais em cultura pt-BR (ex.: "1877,72") e o identificador do grupo

#### Scenario: Bandeira indisponível
- **WHEN** o arquivo de imagem da bandeira não pode ser carregado (asset ausente ou erro de carregamento)
- **THEN** a linha degrada com elegância, sem imagem quebrada, mantendo nome/código da seleção legíveis

#### Scenario: Pontuação fiel à fonte oficial
- **WHEN** a pontuação FIFA de uma seleção é exibida
- **THEN** o valor corresponde exatamente ao definido em `fontes/copa2026_ranking_fifa.txt` (via seed), sem arredondamentos inventados nem valores fabricados

#### Scenario: Barra de pontuação é apenas estética
- **WHEN** a linha exibe a barra de progresso de pontuação (como no protótipo)
- **THEN** essa barra é um elemento **puramente visual**, cuja largura é derivada por fórmula relativa, sem representar nenhum dado oficial; o valor numérico exibido permanece o da fonte. A barra é opcional e pode ser omitida sem violar requisitos

### Requirement: Origem exclusiva dos dados

O ranking SHALL ser alimentado exclusivamente pelos dados oficiais de `fontes/copa2026_ranking_fifa.txt` (refletidos no seed `DadosCopa.Ranking`). O sistema NÃO deve inventar seleções, posições ou pontuações, e não deve incluir seleções ausentes da Copa.

#### Scenario: Apenas seleções da Copa presentes no arquivo
- **WHEN** o ranking é carregado
- **THEN** somente seleções presentes no arquivo de ranking E participantes da Copa 2026 são exibidas (seleções como Itália e Dinamarca, que não disputam a Copa, não aparecem)

#### Scenario: Ausência de dados inventados
- **WHEN** o ranking é montado
- **THEN** nenhuma seleção, posição ou pontuação fora do arquivo oficial é gerada ou estimada

### Requirement: Destaque das três primeiras posições

O sistema SHALL destacar visualmente as três primeiras seleções do ranking num componente de pódio, com cores distintas por medalha — **ouro (1º), prata (2º) e bronze (3º)** — conforme o protótipo. Cada card do pódio SHALL exibir a posição (medalha), a bandeira, o nome da seleção, o grupo e a pontuação FIFA.

#### Scenario: Top 3 destacado
- **WHEN** a página de ranking é exibida sem termo de pesquisa
- **THEN** as seleções nas posições 1, 2 e 3 são exibidas no pódio com cores ouro/prata/bronze respectivamente, mostrando bandeira, nome, grupo e pontuação, e as mesmas seleções também aparecem destacadas nas primeiras linhas da tabela

#### Scenario: Top 3 durante a pesquisa
- **WHEN** há um termo de pesquisa ativo no campo de busca
- **THEN** o componente de pódio do Top 3 deixa de ser exibido e apenas a tabela filtrada (que pode ou não conter seleções do Top 3, conforme o termo) é apresentada

### Requirement: Pesquisa de seleções

O sistema SHALL permitir pesquisar seleções no ranking por nome, filtrando a lista exibida conforme o texto informado, em tempo real, sem recarregar a página.

#### Scenario: Filtrar por nome
- **WHEN** o usuário digita um texto no campo de pesquisa
- **THEN** a lista do ranking exibe apenas as seleções cujo nome corresponde ao texto informado, mantendo a ordem por posição

#### Scenario: Pesquisa sem correspondência
- **WHEN** o texto pesquisado não corresponde a nenhuma seleção
- **THEN** a lista fica vazia e o usuário recebe indicação de que nenhuma seleção foi encontrada

#### Scenario: Limpar pesquisa
- **WHEN** o usuário apaga o texto do campo de pesquisa
- **THEN** a lista volta a exibir todas as seleções do ranking

### Requirement: Componentes reutilizáveis

A interface do ranking SHALL ser decomposta em componentes Razor reutilizáveis, evitando concentrar toda a implementação na página, com componentes específicos para o Top 3, a tabela de ranking, a linha da tabela e a pesquisa.

#### Scenario: Composição por componentes
- **WHEN** a página de ranking é implementada
- **THEN** ela é composta por componentes distintos para destaque do Top 3, tabela de ranking, linha de ranking e campo de pesquisa, sem lógica de acesso a dados duplicada na página

### Requirement: Acesso a dados via serviço dedicado

O acesso aos dados do ranking SHALL ocorrer exclusivamente por meio de um serviço dedicado (`IRankingService`), sem acesso direto ao `DbContext` em páginas ou componentes Razor. O serviço entrega posição, seleção, código/bandeira, pontuação e grupo.

#### Scenario: Página consome o serviço
- **WHEN** a página de ranking precisa dos dados
- **THEN** ela obtém os dados através de `IRankingService` injetado, e nenhum componente Razor acessa o `DbContext` diretamente

#### Scenario: Dados incluem o grupo
- **WHEN** o serviço retorna um item do ranking
- **THEN** o item inclui o grupo da seleção, permitindo a exibição da coluna de grupo

### Requirement: Navegação para o Ranking

O menu de navegação principal do app SHALL conter um item "Ranking" que leva à rota `/ranking`, substituindo o link anterior que apontava para a âncora do gráfico na Landing Page, e SHALL indicar o estado ativo quando a rota atual for `/ranking`.

#### Scenario: Link no menu leva à página
- **WHEN** o usuário clica no item "Ranking" do menu (ou do rodapé)
- **THEN** a aplicação navega para a rota `/ranking` (e não para a âncora `/#ranking` da Landing Page)

#### Scenario: Estado ativo do menu
- **WHEN** a rota atual é `/ranking`
- **THEN** o item "Ranking" do menu é exibido como ativo, consistentemente com os demais itens (Jogos, Grupos, Seleções, Simulador)

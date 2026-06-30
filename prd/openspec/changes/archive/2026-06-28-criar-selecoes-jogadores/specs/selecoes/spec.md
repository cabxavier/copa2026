## ADDED Requirements

### Requirement: Página Equipes composta por componentes
A capacidade de seleções SHALL ser entregue por uma página `Equipes` com rota `/equipes` e render mode interativo (`InteractiveServer`, necessário para busca, filtro e modal reativos), composta por componentes Blazor reutilizáveis residentes em `Components/Pages/Selecoes/`: `Equipes`, `CartaoTime`, `FiltroTime`, `ModalTime` e `TabelaElencoTime`. A página NÃO SHALL concentrar markup e lógica em um único arquivo nem duplicar a renderização de um card de seleção ou de uma linha de jogador.

#### Scenario: Página acessível por rota
- **WHEN** o usuário navega para `/equipes`
- **THEN** a página `Equipes` é renderizada com o título, a busca, o filtro de grupos e a grade de seleções

#### Scenario: Componentes isolados em arquivos próprios
- **WHEN** o código da página de equipes é inspecionado
- **THEN** o card está em `CartaoTime`, o filtro/busca em `FiltroTime`, o modal em `ModalTime` e a tabela de elenco em `TabelaElencoTime`, todos em `Components/Pages/Selecoes/`

### Requirement: Listagem das 48 seleções
A página SHALL exibir as 48 seleções participantes a partir dos dados oficiais persistidos (`/fontes`), sem gerar dados fictícios nem inventar seleções. Cada seleção SHALL ser apresentada por um `CartaoTime` contendo a bandeira, o nome da seleção, o grupo (ex.: "Grupo A") e o código FIFA (ex.: "BRA"). As seleções SHALL ser ordenadas alfabeticamente por nome.

#### Scenario: Exibição das 48 seleções
- **WHEN** a página de equipes é carregada sem busca nem filtro aplicados
- **THEN** as 48 seleções são exibidas em cards, cada um com bandeira, nome, grupo e código FIFA

#### Scenario: Ordenação alfabética
- **WHEN** a grade de seleções é renderizada
- **THEN** os cards aparecem em ordem alfabética crescente pelo nome da seleção

#### Scenario: Ausência de dados inventados
- **WHEN** as seleções exibidas são comparadas com os dados de `/fontes`
- **THEN** não há seleções, grupos ou códigos além dos definidos nos dados oficiais

### Requirement: Busca por seleção
A página SHALL permitir buscar seleções pelo nome por meio de um campo de texto. O filtro SHALL ser case-insensitive e por correspondência parcial (substring), atualizando a grade conforme o usuário digita. Quando nenhuma seleção corresponder, a página SHALL exibir uma mensagem de "nenhuma seleção encontrada".

#### Scenario: Filtrar por nome
- **WHEN** o usuário digita parte do nome de uma seleção no campo de busca
- **THEN** a grade passa a exibir somente as seleções cujo nome contém o texto informado, ignorando maiúsculas/minúsculas

#### Scenario: Busca sem resultados
- **WHEN** o texto de busca não corresponde a nenhuma seleção
- **THEN** a página exibe uma mensagem indicando que nenhuma seleção foi encontrada

### Requirement: Filtro por grupo
A página SHALL oferecer um filtro por grupo com chips no estilo do protótipo: uma opção "Todos" e uma para cada um dos 12 grupos, rotuladas "Grp A" a "Grp L". Ao selecionar um grupo, a grade SHALL exibir apenas as seleções daquele grupo. A busca e o filtro por grupo SHALL ser combináveis (aplicados em conjunto). A opção "Todos" SHALL remover o filtro de grupo.

#### Scenario: Filtrar por um grupo específico
- **WHEN** o usuário seleciona o chip de um grupo (ex.: "Grp C")
- **THEN** apenas as seleções daquele grupo são exibidas, e o chip selecionado aparece como ativo

#### Scenario: Combinar busca e filtro
- **WHEN** há um grupo selecionado e um texto de busca informado
- **THEN** a grade exibe somente as seleções que pertencem ao grupo e cujo nome corresponde à busca

#### Scenario: Remover o filtro de grupo
- **WHEN** o usuário seleciona o chip "Todos"
- **THEN** a grade volta a considerar seleções de todos os grupos (respeitando a busca, se houver)

### Requirement: Modal de elenco da seleção
Ao selecionar uma seleção, a página SHALL abrir um modal (`ModalTime`) exibindo a bandeira, o nome da seleção, o grupo, o nome do técnico e a posição no Ranking FIFA. O modal SHALL conter a tabela do elenco (`TabelaElencoTime`) com os jogadores convocados, cada um com nome, posição, idade e gols pela seleção. O modal SHALL poder ser fechado pelo botão de fechar e ao clicar fora da caixa do modal. Quando a seleção não possuir posição no Ranking FIFA (cobertura parcial dos dados oficiais), o modal SHALL indicar a ausência (ex.: "Sem ranking") em vez de inventar um valor.

#### Scenario: Abertura do modal
- **WHEN** o usuário aciona um `CartaoTime`
- **THEN** o modal abre exibindo bandeira, nome, grupo, técnico e ranking FIFA da seleção, e a tabela do elenco convocado

#### Scenario: Linha de jogador
- **WHEN** a tabela de elenco é renderizada
- **THEN** cada jogador é exibido com nome, posição, idade e gols pela seleção, com a posição destacada por uma etiqueta colorida

#### Scenario: Seleção sem ranking oficial
- **WHEN** o modal é aberto para uma seleção sem posição no Ranking FIFA
- **THEN** o campo de ranking indica a ausência (ex.: "Sem ranking"), sem exibir número inventado

#### Scenario: Fechamento do modal
- **WHEN** o usuário aciona o botão de fechar ou clica fora da caixa do modal
- **THEN** o modal é fechado e a grade de seleções permanece visível

### Requirement: Seed de jogadores e técnicos a partir de /fontes
O sistema SHALL popular os jogadores convocados e o técnico de cada seleção a partir dos arquivos oficiais `copa2026_selecoes_jogadores.txt` e `copa2026_pais_tecnicos.txt` de `/fontes`, vinculando cada registro à seleção pelo código FIFA (`Selecao.Codigo`). Cada jogador SHALL ter nome, posição, idade e gols pela seleção transcritos fielmente da fonte; a posição textual da fonte ("Goleiro", "Defensor", "Meio-campista", "Atacante") SHALL ser mapeada para o enum `PosicaoJogador`. A carga SHALL ser idempotente (não duplica em execuções subsequentes) e NÃO SHALL inventar jogadores, idades, gols ou técnicos.

#### Scenario: Elenco populado por seleção
- **WHEN** a aplicação inicializa o banco com as tabelas vazias
- **THEN** cada seleção recebe seu elenco (aproximadamente 26 jogadores) e o nome do seu técnico, conforme `/fontes`

#### Scenario: Vínculo por código FIFA
- **WHEN** um jogador ou técnico é associado a uma seleção
- **THEN** o vínculo usa o código FIFA da seleção, independentemente de diferenças de idioma no nome entre os arquivos de origem

#### Scenario: Idempotência da carga
- **WHEN** a aplicação é reiniciada com o banco já populado
- **THEN** nenhum jogador ou técnico é duplicado

### Requirement: Acesso pela navegação
O menu principal (`Components/Layout/MainLayout.razor`) SHALL incluir um item de seleções (rótulo "Seleções" ou "Equipes") que navega para `/equipes`, substituindo o estado "em breve" atual. O item correspondente à rota atual SHALL ser marcado como ativo.

#### Scenario: Item de menu para equipes
- **WHEN** o usuário visualiza o menu principal
- **THEN** existe um item de seleções que, ao ser acionado, navega para `/equipes`

#### Scenario: Item ativo reflete a rota atual
- **WHEN** o usuário está na rota `/equipes`
- **THEN** o item de seleções aparece como ativo

### Requirement: Acesso a dados por serviço
Os componentes da página de equipes SHALL obter dados exclusivamente por meio de um serviço injetado (`SelecaoService`, camada `Services/`) que retorna DTOs, e NÃO SHALL executar consultas EF Core diretamente nos arquivos `.razor`.

#### Scenario: Componente consome serviço
- **WHEN** um componente da página de equipes precisa de dados
- **THEN** ele recebe o `SelecaoService` via injeção de dependência e chama métodos assíncronos que retornam DTOs

#### Scenario: Ausência de EF Core nos componentes
- **WHEN** o código dos componentes da página de equipes é inspecionado
- **THEN** não há uso direto de `AppDbContext` ou consultas EF Core nos arquivos `.razor`

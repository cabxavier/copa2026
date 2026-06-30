# jogos Specification

## Purpose
TBD - created by archiving change criar-jogos-grupos. Update Purpose after archive.
## Requirements
### Requirement: Página de jogos composta por componentes
A capacidade de jogos SHALL ser entregue por uma página `Jogos` com rota `/jogos` e render mode interativo (`InteractiveServer`, necessário para o filtro reativo), composta por componentes Blazor reutilizáveis residentes em `Components/Pages/Jogos/`: `Jogos`, `JogosFiltro`, `JogosGrupoHeader` e `JogoCard`. A página NÃO SHALL concentrar markup e lógica em um único arquivo nem duplicar a renderização de uma partida.

#### Scenario: Página acessível por rota
- **WHEN** o usuário navega para `/jogos`
- **THEN** a página `Jogos` é renderizada com a listagem de partidas e o filtro de grupos

#### Scenario: Interatividade do filtro habilitada
- **WHEN** a página `Jogos` é inspecionada
- **THEN** ela usa render mode interativo (`InteractiveServer`), permitindo que a troca de filtro recarregue a listagem sem recarregar a página

#### Scenario: Componentes isolados em arquivos próprios
- **WHEN** o código da página de jogos é inspecionado
- **THEN** o filtro está em `JogosFiltro`, o cabeçalho de grupo em `JogosGrupoHeader` e cada partida em `JogoCard`, todos em `Components/Pages/Jogos/`

### Requirement: Listagem de jogos a partir dos dados oficiais
A página SHALL listar exclusivamente os jogos carregados via Seed Data (dados oficiais de `/fontes`), sem gerar dados fictícios, inventar confrontos ou exibir grupos não definidos.

#### Scenario: Exibição dos jogos semeados
- **WHEN** a página de jogos é carregada sem filtro aplicado
- **THEN** todos os jogos da fase de grupos persistidos são exibidos, cada um com mandante e visitante

#### Scenario: Ausência de dados inventados
- **WHEN** os jogos exibidos são comparados com os dados de `/fontes`
- **THEN** não há partidas, seleções ou grupos além dos definidos nos dados oficiais

### Requirement: Agrupamento por grupo
A listagem SHALL agrupar os jogos pelo grupo do torneio, exibindo os grupos em ordem alfabética (Grupo A a Grupo L). Dentro de cada grupo, os jogos SHALL ser ordenados por data e horário de forma crescente; como os dados oficiais semeados passam a incluir o horário da partida, a ordem entre jogos do mesmo dia SHALL refletir o horário real, com desempate estável e determinístico por `Id` quando dois jogos tiverem exatamente a mesma data e hora. Cada grupo SHALL ser introduzido por um cabeçalho (`JogosGrupoHeader`) com o título do grupo ("Grupo X · Fase de grupos") e a quantidade de jogos.

#### Scenario: Jogos agrupados por grupo
- **WHEN** a listagem é renderizada sem filtro
- **THEN** os jogos aparecem agrupados por grupo, do Grupo A ao Grupo L, cada grupo sob um cabeçalho com o título e a contagem de jogos

#### Scenario: Ordem cronológica dentro do grupo
- **WHEN** existem jogos em datas e horários diferentes dentro de um grupo
- **THEN** eles são exibidos em ordem crescente de data e horário, com desempate determinístico e estável por `Id` quando a data e a hora forem idênticas

### Requirement: Filtro por grupo
O componente `JogosFiltro` SHALL permitir filtrar a listagem pelos 12 grupos do torneio (Grupo A a Grupo L). As opções de grupo SHALL ser obtidas dos dados oficiais via serviço, e cada rótulo SHALL usar o nome persistido do grupo (`Grupo.Nome`, ex.: "Grupo A"). Ao aplicar um filtro, a listagem SHALL exibir somente os jogos do grupo selecionado, mantendo o agrupamento por grupo e a ordenação. SHALL existir uma opção para exibir todos os grupos.

#### Scenario: Filtrar por um grupo específico
- **WHEN** o usuário seleciona um grupo no `JogosFiltro`
- **THEN** apenas os jogos daquele grupo são exibidos, sob o cabeçalho do grupo e ordenados por data

#### Scenario: Remover o filtro
- **WHEN** o usuário seleciona a opção de todos os grupos
- **THEN** a listagem volta a exibir os jogos de todos os grupos

#### Scenario: Opções de grupo a partir dos dados oficiais
- **WHEN** o `JogosFiltro` é renderizado
- **THEN** as opções de grupo correspondem aos grupos persistidos (Grupo A a Grupo L, pelo `Grupo.Nome`), sem grupos inventados

### Requirement: Informações de grupo e estádio na partida
Cada partida exibida por `JogoCard` SHALL apresentar as seleções mandante e visitante (nome e bandeira), o **horário** da partida (formato `HH:mm`), o grupo da partida e as informações do estádio (nome do estádio e cidade). O horário SHALL ser exibido junto ao nome do grupo na label da partida (ex.: "16:00 Grupo A").

#### Scenario: Exibição das informações da partida
- **WHEN** um `JogoCard` é renderizado
- **THEN** ele mostra o mandante e o visitante com suas bandeiras, o horário no formato `HH:mm` junto ao grupo, e o estádio com a cidade

#### Scenario: Horário a partir dos dados oficiais
- **WHEN** o horário de uma partida é exibido
- **THEN** ele corresponde ao horário oficial semeado a partir de `/fontes`, sem exibir "00:00" por ausência de horário

### Requirement: Exibição do resultado oficial persistido
Cada partida exibida por `JogoCard` SHALL refletir o placar oficial persistido na entidade `Jogo` (`GolsMandante`/`GolsVisitante`), gravado pela página Grupos. Considera-se que há resultado quando ambos os gols estão preenchidos (não nulos). Quando há resultado, o `JogoCard` SHALL exibir o status "ENCERRADO" e o placar real (ex.: "2 : 1"); quando não há resultado, SHALL exibir o status "AGENDADO" e o placar vazio ("— : —"). A página de jogos NÃO SHALL consultar resultados do Simulador (`SimulacaoJogo`); a fonte do placar é exclusivamente a tabela `Jogo`. O placar é lido a cada carregamento da página, refletindo o último valor persistido.

#### Scenario: Partida com resultado oficial
- **WHEN** um `JogoCard` é renderizado para um jogo cujo placar oficial foi gravado (ambos os gols preenchidos)
- **THEN** ele exibe o status "ENCERRADO" e o placar persistido (`GolsMandante` : `GolsVisitante`)

#### Scenario: Partida sem resultado oficial
- **WHEN** um `JogoCard` é renderizado para um jogo sem placar oficial (um ou ambos os gols nulos)
- **THEN** ele exibe o status "AGENDADO" e o placar vazio "— : —"

#### Scenario: Reflexo do resultado gravado em Grupos
- **WHEN** um resultado é salvo na página Grupos e a página de jogos é carregada (ou recarregada) em seguida
- **THEN** o placar exibido na listagem de jogos corresponde ao valor persistido na tabela `Jogo`

### Requirement: Botão Ver Grupos
A página de jogos SHALL exibir um botão "Ver Grupos" como chamada de ação para a área de grupos. Como a página de grupos está fora do escopo desta change e ainda não existe, o botão SHALL ser exibido em estado "em breve" (desabilitado, sem navegação), evitando link quebrado.

#### Scenario: Botão em estado "em breve"
- **WHEN** a página de jogos é renderizada e a página de grupos ainda não existe
- **THEN** o botão "Ver Grupos" é exibido em estado desabilitado/"em breve", sem disparar navegação nem produzir link quebrado

### Requirement: Acesso pela navegação
O menu de navegação (`Components/Layout/NavMenu.razor`) SHALL incluir um item "Jogos" que direcione para a rota `/jogos`. O menu do cabeçalho (`Components/Layout/MainLayout.razor`) SHALL conter um item "Jogos" apontando para `/jogos`, e o item correspondente à rota atual SHALL ser marcado como ativo (e os demais desmarcados).

#### Scenario: Item de menu para jogos
- **WHEN** o usuário visualiza o menu de navegação
- **THEN** existe um item "Jogos" que, ao ser acionado, navega para `/jogos`

#### Scenario: Item ativo reflete a rota atual
- **WHEN** o usuário está na rota `/jogos`
- **THEN** o item "Jogos" do cabeçalho aparece como ativo e o item "Home" não fica marcado

### Requirement: Acesso a dados por serviço
Os componentes da página de jogos SHALL obter dados exclusivamente por meio de um serviço injetado (`JogosService`, camada `Services/`) que retorna DTOs, e NÃO SHALL executar consultas EF Core diretamente nos arquivos `.razor`.

#### Scenario: Componente consome serviço
- **WHEN** um componente da página de jogos precisa de dados
- **THEN** ele recebe o serviço via injeção de dependência e chama métodos assíncronos que retornam DTOs

#### Scenario: Ausência de EF Core nos componentes
- **WHEN** o código dos componentes da página de jogos é inspecionado
- **THEN** não há uso direto de `AppDbContext` ou consultas EF Core nos arquivos `.razor`


## ADDED Requirements

### Requirement: Página de Simulador

O sistema SHALL disponibilizar a página `/simulador` em render mode interativo (Interactive Server), acessível pela navegação principal, exibindo um grupo por vez (A a L) com seus jogos, a classificação simulada e as seleções do grupo. Os componentes `.razor` da página NÃO SHALL acessar `AppDbContext` diretamente; todo acesso a dados ocorre via serviço.

#### Scenario: Acesso à página
- **WHEN** o usuário navega para `/simulador`
- **THEN** a página carrega o grupo A por padrão, exibindo seus jogos (6 por grupo) com campos de placar, a classificação simulada e a lista de seleções do grupo

#### Scenario: Item de navegação ativo
- **WHEN** o usuário está em `/simulador`
- **THEN** o item "Simulador" do cabeçalho/menu é destacado como ativo

### Requirement: Seleção de grupo

O sistema SHALL apresentar chips dos 12 grupos (A–L) permitindo alternar o grupo exibido, mantendo o grupo selecionado destacado.

#### Scenario: Alternar grupo
- **WHEN** o usuário clica no chip de um grupo
- **THEN** os jogos, a classificação e a lista de seleções passam a refletir o grupo escolhido, e o chip selecionado fica destacado

### Requirement: Entrada de placares

O sistema SHALL exibir, para cada jogo do grupo (6 por grupo, round-robin de 4 seleções), os nomes e bandeiras das seleções mandante e visitante e dois campos numéricos de placar (mandante e visitante). Os campos SHALL aceitar apenas inteiros não negativos; valores fora do intervalo válido SHALL ser normalizados. Um jogo só entra no cálculo quando ambos os placares estiverem preenchidos.

#### Scenario: Informar um placar
- **WHEN** o usuário digita gols do mandante e do visitante de um jogo
- **THEN** o jogo passa a contar na classificação e a tabela é recalculada automaticamente

#### Scenario: Placar parcial não computa
- **WHEN** apenas um dos dois campos de um jogo está preenchido
- **THEN** o jogo é ignorado no cálculo da classificação

#### Scenario: Valor inválido normalizado
- **WHEN** o usuário informa um valor negativo ou não numérico
- **THEN** o sistema normaliza para um inteiro válido (mínimo 0) antes de computar

### Requirement: Cálculo da classificação

O sistema SHALL calcular, para o grupo exibido, a classificação com as colunas Jogos (J), Vitórias (V), Empates (E), Derrotas (D), Saldo de Gols (SG) e Pontos (PTS), atribuindo 3 pontos por vitória, 1 por empate e 0 por derrota, considerando apenas jogos com ambos os placares preenchidos.

#### Scenario: Pontuação por resultado
- **WHEN** um jogo tem placar com vencedor
- **THEN** o vencedor recebe 3 pontos e o perdedor 0; em empate, cada seleção recebe 1 ponto, e as colunas V/E/D, gols e saldo são atualizadas

#### Scenario: Recálculo em tempo real
- **WHEN** qualquer placar do grupo é alterado
- **THEN** a classificação do grupo é recalculada e reexibida imediatamente

### Requirement: Critérios de desempate

O sistema SHALL ordenar a classificação aplicando, em ordem, os critérios oficiais de desempate dentro do grupo: (1) maior número de pontos; (2) maior saldo de gols; (3) maior número de gols marcados; (4) confronto direto entre as seleções empatadas. Quando **três ou mais** seleções permanecerem empatadas após os critérios 1–3, o critério 4 SHALL ser aplicado como **mini-tabela (sub-grupo)** considerando apenas os jogos entre as seleções empatadas, reaplicando pontos → saldo de gols → gols marcados dentro desse sub-grupo. Persistindo o empate, ou quando não houver jogos suficientes entre as empatadas para decidir, a ordenação SHALL recair em uma ordem estável e determinística (por `SelecaoId`).

#### Scenario: Desempate por saldo e gols
- **WHEN** duas seleções têm os mesmos pontos
- **THEN** fica à frente a de maior saldo de gols; persistindo o empate, a de maior número de gols marcados

#### Scenario: Desempate por confronto direto entre duas seleções
- **WHEN** duas seleções permanecem empatadas em pontos, saldo e gols marcados e o confronto direto entre elas tem placar preenchido
- **THEN** fica à frente a vencedora desse confronto direto

#### Scenario: Desempate entre três ou mais seleções (mini-tabela)
- **WHEN** três ou mais seleções permanecem empatadas em pontos, saldo e gols marcados
- **THEN** o sistema monta uma mini-tabela apenas com os jogos entre elas e reaplica pontos → saldo → gols marcados para ordená-las

#### Scenario: Empate indecidível recai em ordem estável
- **WHEN** o confronto direto não pode decidir (sem placar suficiente entre as empatadas)
- **THEN** a ordem entre elas é determinística e estável (por `SelecaoId`), sem oscilar entre recálculos

### Requirement: Destaque de classificados e wildcard

O sistema SHALL destacar visualmente os 2 primeiros colocados de cada grupo como classificados e o 3º colocado como candidato a wildcard, apresentando uma legenda que identifique cada destaque.

#### Scenario: Realce das posições
- **WHEN** a classificação é exibida
- **THEN** as 2 primeiras linhas recebem o destaque de "Classifica" e a 3ª linha o destaque de "Wildcard", conforme a legenda

### Requirement: Limpar resultados

O sistema SHALL oferecer as ações "Limpar Grupo" (zera os placares apenas do grupo exibido) e "Limpar Tudo" (zera os placares de todos os grupos), recalculando a classificação e persistindo o novo estado.

#### Scenario: Limpar grupo
- **WHEN** o usuário aciona "Limpar Grupo"
- **THEN** os placares do grupo atual são apagados, a classificação é zerada e o estado é persistido

#### Scenario: Limpar tudo
- **WHEN** o usuário aciona "Limpar Tudo"
- **THEN** os placares de todos os grupos são apagados e o estado é persistido

### Requirement: Tabela responsiva e legível

O sistema SHALL renderizar a classificação com tabela Bootstrap 5 responsiva, com colunas alinhadas (numéricas centralizadas/à direita), boa legibilidade e comportamento adequado em telas pequenas.

#### Scenario: Visualização em tela pequena
- **WHEN** a página é exibida em viewport estreito
- **THEN** a tabela permanece legível e utilizável (sem quebra de alinhamento das colunas), com rolagem horizontal quando necessário

### Requirement: Persistência automática da simulação corrente

O sistema SHALL manter automaticamente uma única simulação corrente persistida em SQLite, sem exigir que o usuário informe um nome. Cada alteração de placar (incluindo as ações de limpar) SHALL ser persistida automaticamente. Ao retornar à página, a simulação corrente SHALL ser restaurada com todos os placares previamente informados.

#### Scenario: Salvamento automático ao editar
- **WHEN** o usuário altera ou limpa um placar
- **THEN** o resultado é gravado na simulação corrente sem ação explícita de salvar

#### Scenario: Restauração ao retornar
- **WHEN** o usuário sai e retorna a `/simulador` (inclusive após reiniciar a aplicação)
- **THEN** os placares previamente informados são restaurados e a classificação reflete esse estado

#### Scenario: Sem nome exigido
- **WHEN** a simulação corrente é criada ou atualizada
- **THEN** o sistema não solicita nome ao usuário, usando um identificador padrão interno para a simulação corrente

### Requirement: Limite de escopo à fase de grupos

O sistema SHALL limitar a simulação à fase de grupos. NÃO SHALL calcular os 8 melhores terceiros entre grupos nem qualquer etapa de mata-mata.

#### Scenario: Sem mata-mata
- **WHEN** o usuário utiliza o simulador
- **THEN** apenas a classificação interna de cada grupo é apresentada, sem chaveamento de mata-mata nem ranking de terceiros entre grupos

## ADDED Requirements

### Requirement: Página de Grupos

O sistema SHALL disponibilizar a página `/grupos` em render mode interativo (Interactive Server), acessível pela navegação principal, exibindo um grupo por vez (A a L) com sua classificação oficial, seus jogos e a possibilidade de registrar resultados oficiais. Os componentes `.razor` da página NÃO SHALL acessar `AppDbContext` diretamente; todo acesso a dados ocorre via `IGruposService`. A página SHALL manter fidelidade visual ao protótipo `../prototipo/grupos.html`, reproduzindo sua estrutura: cabeçalho da página (`page-head`) com título "Grupos", subtítulo e botão "Simular" à direita; chips de grupos; e layout em duas colunas (`two-col`) com os jogos à esquerda (`glist`/`grow`) e a classificação à direita, com a legenda no rodapé do painel de classificação. Os estilos próprios da página SHALL residir em `wwwroot/css/portal.css`, escopados em `.grupos` quando compartilharem classes com outras telas, sem alterar a aparência do Simulador ou da página Jogos.

#### Scenario: Acesso à página
- **WHEN** o usuário navega para `/grupos`
- **THEN** a página carrega o grupo A por padrão, exibindo sua classificação oficial, a lista de jogos do grupo e a legenda de classificação

#### Scenario: Item de navegação ativo
- **WHEN** o usuário está em `/grupos`
- **THEN** o item "Grupos" do cabeçalho/menu é destacado como ativo

### Requirement: Navegação por grupos

O sistema SHALL apresentar chips dos 12 grupos (A–L), rotulados com o nome completo do grupo (ex.: "Grupo A"), permitindo alternar o grupo exibido e mantendo o chip do grupo selecionado destacado. Ao trocar de grupo, os títulos dos painéis SHALL refletir o grupo escolhido (ex.: "Classificação — Grupo B", "Jogos — Grupo B").

#### Scenario: Alternar grupo
- **WHEN** o usuário clica no chip de um grupo
- **THEN** a classificação, os jogos e os títulos dos painéis passam a refletir o grupo escolhido, e o chip selecionado fica destacado

### Requirement: Classificação oficial do grupo

O sistema SHALL exibir a classificação do grupo selecionado calculada dinamicamente a partir dos resultados oficiais armazenados na tabela `Jogos`, com as colunas: Posição (#), Seleção (nome e bandeira), Jogos (J), Vitórias (V), Empates (E), Derrotas (D), Gols Pró (GP), Gols Contra (GC), Saldo de Gols (SG) e Pontos (PTS). A pontuação SHALL ser 3 por vitória, 1 por empate e 0 por derrota. Apenas jogos com **ambos** os placares oficiais preenchidos SHALL ser computados. As 4 seleções do grupo SHALL aparecer sempre, mesmo sem jogos computados (linhas zeradas).

#### Scenario: Pontuação por resultado oficial
- **WHEN** um jogo oficial tem placar com vencedor
- **THEN** o vencedor recebe 3 pontos e o perdedor 0; em empate cada seleção recebe 1 ponto, e as colunas J/V/E/D, GP, GC e SG são atualizadas conforme os gols

#### Scenario: Jogo sem resultado não computa
- **WHEN** um jogo do grupo ainda não tem placar oficial completo
- **THEN** ele não é contabilizado na classificação, e as seleções permanecem com as estatísticas dos demais jogos

#### Scenario: Grupo sem resultados
- **WHEN** nenhum jogo do grupo possui placar oficial
- **THEN** as 4 seleções aparecem com todas as estatísticas zeradas

### Requirement: Critérios de desempate da classificação oficial

O sistema SHALL ordenar a classificação aplicando, em ordem, os critérios oficiais de desempate dentro do grupo definidos em `fontes/copa2026_regras_negocio.txt`: (1) maior número de pontos; (2) maior saldo de gols; (3) maior número de gols marcados; (4) confronto direto entre as seleções empatadas. Quando três ou mais seleções permanecerem empatadas após os critérios 1–3, o critério de confronto direto SHALL ser aplicado como mini-tabela (sub-grupo) considerando apenas os jogos entre as empatadas, reaplicando pontos → saldo → gols marcados. Persistindo o empate (ou sem jogos suficientes para decidir, incluindo os critérios não modelados de fair play/cartões e sorteio), a ordenação SHALL recair em uma ordem estável e determinística (por `SelecaoId`).

#### Scenario: Desempate por saldo e gols
- **WHEN** duas seleções têm os mesmos pontos
- **THEN** fica à frente a de maior saldo de gols; persistindo o empate, a de maior número de gols marcados

#### Scenario: Desempate por confronto direto
- **WHEN** duas seleções permanecem empatadas em pontos, saldo e gols marcados e o confronto direto entre elas tem placar oficial
- **THEN** fica à frente a vencedora desse confronto direto

#### Scenario: Empate indecidível recai em ordem estável
- **WHEN** o confronto direto não pode decidir o empate
- **THEN** a ordem entre as seleções é determinística e estável (por `SelecaoId`), sem oscilar entre recálculos

### Requirement: Destaque de classificados, melhor terceiro e eliminados

O sistema SHALL destacar visualmente os 2 primeiros colocados de cada grupo como classificados (verde), o 3º colocado como melhor terceiro (laranja) e os colocados a partir do 4º como eliminados (cinza). A legenda SHALL ser exibida **uma única vez**, no rodapé do painel de classificação (conforme o protótipo), com os textos "Classificado (1º e 2º de cada grupo)", "Melhor Terceiro (8 melhores avançam)" e "Eliminado"; a página NÃO SHALL renderizar uma segunda legenda.

#### Scenario: Realce das posições
- **WHEN** a classificação é exibida
- **THEN** as 2 primeiras linhas recebem o destaque de classificado, a 3ª linha o destaque de melhor terceiro e as linhas a partir da 4ª o destaque de eliminado, conforme a legenda do painel

#### Scenario: Legenda única
- **WHEN** a página Grupos é exibida
- **THEN** existe exatamente uma legenda, no rodapé do painel de classificação, com os textos "Classificado (1º e 2º de cada grupo)", "Melhor Terceiro (8 melhores avançam)" e "Eliminado", sem legenda duplicada

### Requirement: Jogos do grupo

O sistema SHALL exibir, no painel de jogos do grupo selecionado, a lista dos jogos da fase de grupos daquele grupo (mandante e visitante com bandeiras, data/horário), ordenados cronologicamente, exibindo o placar oficial quando registrado.

#### Scenario: Listar jogos do grupo
- **WHEN** um grupo é selecionado
- **THEN** os jogos desse grupo são listados em ordem cronológica, com nomes e bandeiras das seleções e o placar oficial quando houver

### Requirement: Registro e atualização de resultados oficiais

O sistema SHALL permitir registrar e atualizar o resultado oficial de cada jogo da fase de grupos diretamente na página, com campos numéricos de placar (mandante e visitante) que aceitam apenas inteiros não negativos, normalizados à faixa `0..99`; valores inválidos SHALL ser normalizados para inteiros válidos (mínimo 0). A gravação SHALL ocorrer por **ação explícita de salvar** (botão de confirmação por jogo), e NÃO automaticamente a cada digitação — distinta do Simulador, por se tratar de resultado oficial. Ao salvar um resultado com ambos os gols, o sistema SHALL persistir os gols na tabela `Jogos`; quando um placar for tornado incompleto, o resultado oficial daquele jogo SHALL ser removido. Após salvar, a classificação do grupo e as estatísticas das seleções envolvidas SHALL ser recalculadas e refletidas imediatamente na interface.

#### Scenario: Salvar resultado oficial
- **WHEN** o usuário informa os gols do mandante e do visitante de um jogo e aciona a ação de salvar daquele jogo
- **THEN** os gols são gravados na tabela `Jogos`, a classificação é recalculada, a interface é atualizada imediatamente e o jogo exibe uma confirmação visual "✓ salvo" temporária

#### Scenario: Gravação só por ação explícita
- **WHEN** o usuário digita um placar mas não aciona a ação de salvar
- **THEN** nenhum resultado oficial é gravado na tabela `Jogos`

#### Scenario: Resultado persistido entre execuções
- **WHEN** um resultado oficial é salvo e a aplicação é reiniciada
- **THEN** ao retornar a `/grupos` o placar oficial e a classificação dele decorrente permanecem disponíveis

#### Scenario: Valor inválido normalizado
- **WHEN** o usuário informa um valor negativo ou não numérico
- **THEN** o sistema normaliza para um inteiro válido (mínimo 0) antes de gravar

#### Scenario: Placar tornado incompleto
- **WHEN** um dos campos de placar de um jogo previamente registrado é esvaziado
- **THEN** o resultado oficial daquele jogo é removido e ele deixa de contar na classificação

### Requirement: Independência entre classificação oficial e Simulador

O sistema SHALL calcular a classificação da página Grupos utilizando exclusivamente os resultados oficiais da tabela `Jogos`. Os resultados do Simulador (tabela `SimulacaoJogos`) NÃO SHALL interferir na classificação oficial dos grupos, e a edição de resultados oficiais NÃO SHALL alterar os resultados do Simulador.

#### Scenario: Simulador não afeta classificação oficial
- **WHEN** existem placares registrados no Simulador para jogos do grupo
- **THEN** a classificação exibida na página Grupos ignora esses placares simulados e usa apenas os resultados oficiais de `Jogos`

#### Scenario: Resultado oficial não afeta o Simulador
- **WHEN** um resultado oficial é registrado ou atualizado na página Grupos
- **THEN** os placares do Simulador permanecem inalterados

### Requirement: Integração com o Simulador

O sistema SHALL exibir um botão "Simular" no cabeçalho da página (`page-head`) que conduz o usuário à página `/simulador`, conforme o protótipo.

#### Scenario: Navegar para o Simulador
- **WHEN** o usuário aciona o botão "Simular"
- **THEN** o usuário é direcionado para a página `/simulador`

### Requirement: Tabela responsiva e legível

O sistema SHALL renderizar a classificação com tabela Bootstrap 5 responsiva, com colunas numéricas alinhadas e rolagem horizontal quando necessário, mantendo legibilidade em telas pequenas.

#### Scenario: Visualização em tela pequena
- **WHEN** a página é exibida em viewport estreito
- **THEN** a tabela permanece legível e utilizável, com rolagem horizontal quando necessário

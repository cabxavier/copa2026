# persistencia-dados

## MODIFIED Requirements

### Requirement: Modelo de domínio da Copa
O sistema SHALL definir as entidades de domínio `Grupo`, `Selecao`, `Jogador`, `Jogo`, `RankingFifa`, `Simulacao` e `SimulacaoJogo`, com seus atributos e relacionamentos representando a estrutura da Copa do Mundo 2026. O atributo de data do `Jogo` (`Jogo.Data`) SHALL representar a data **e o horário** oficial da partida.

#### Scenario: Relacionamento grupo e seleções
- **WHEN** um `Grupo` é carregado
- **THEN** é possível navegar para as `Selecao` que pertencem a esse grupo

#### Scenario: Relacionamento seleção e jogadores
- **WHEN** uma `Selecao` é carregada
- **THEN** é possível navegar para os `Jogador` que compõem o seu elenco, com nome, posição, idade, gols marcados e participações em copas

#### Scenario: Relacionamento jogo, seleções e estádio
- **WHEN** um `Jogo` é carregado
- **THEN** ele referencia a seleção mandante, a seleção visitante, a data **com horário**, o grupo e as informações de estádio

#### Scenario: Ranking FIFA por seleção
- **WHEN** o `RankingFifa` é consultado
- **THEN** cada registro associa uma `Selecao` à sua pontuação e posição no ranking

### Requirement: Carga de dados iniciais via SeedData
O sistema SHALL popular o banco com os dados oficiais iniciais da Copa por meio de SeedData, de forma idempotente, utilizando **exclusivamente** os dados oficiais da pasta `/fontes` (sem dados fictícios, confrontos inventados ou grupos não definidos). A carga SHALL contemplar, no mínimo, as entidades exibidas na Landing Page: grupos (12), seleções (48), ranking FIFA e jogos da fase de grupos (72, todos com data **e horário**, estádio e cidade). Os jogos SHALL ser semeados com o horário oficial extraído de `copa2026_jogos_primeira_fase.txt` (formato `HHhMM`, ex.: `16h00`), combinado à data e preservando `DateTimeKind.Utc`; nenhum horário SHALL ser inventado. A carga de jogadores e dos jogos do mata-mata permanece opcional/futura.

O ranking FIFA SHALL ser carregado apenas para as seleções presentes em `copa2026_ranking_fifa.txt` (cobertura parcial das 48 — as demais ficam sem registro de ranking, sem valores inventados). Os códigos de seleção (`Codigo`, padrão de 3 letras) e a `BandeiraUrl` SHALL usar os códigos oficiais de país (ISO 3166-1 alpha-3 / código FIFA) — identificadores factuais de país, não dados de torneio — já que `/fontes` não os fornece; o vínculo entre arquivos (ranking ↔ seleção ↔ jogo) SHALL ser feito pelos nomes canônicos de `copa2026_grupos.txt`.

#### Scenario: Seed em banco vazio
- **WHEN** a aplicação inicia com as tabelas de dados oficiais vazias
- **THEN** os dados de grupos, seleções, ranking FIFA, estádios/sedes e jogos da fase de grupos são inseridos a partir dos arquivos de `/fontes`, com cada jogo carregado com data e horário oficiais

#### Scenario: Seed não duplica dados
- **WHEN** a aplicação inicia novamente com os dados oficiais já presentes
- **THEN** o SeedData não insere registros duplicados

#### Scenario: Uso exclusivo dos dados oficiais
- **WHEN** os dados carregados são inspecionados
- **THEN** os 12 grupos, as 48 seleções, os 72 jogos da fase de grupos (com seus horários oficiais) e o ranking FIFA correspondem aos arquivos de `/fontes`, sem confrontos, grupos, seleções ou horários inventados

#### Scenario: Ranking parcial sem invenção
- **WHEN** uma seleção não consta em `copa2026_ranking_fifa.txt`
- **THEN** ela é carregada normalmente, porém sem registro de `RankingFifa`, e nenhum valor de ranking é inventado para ela

# persistencia-dados

## MODIFIED Requirements

### Requirement: Carga de dados iniciais via SeedData
O sistema SHALL popular o banco com os dados oficiais iniciais da Copa por meio de SeedData, de forma idempotente, utilizando **exclusivamente** os dados oficiais da pasta `/fontes` (sem dados fictícios, confrontos inventados ou grupos não definidos). A carga SHALL contemplar, no mínimo, as entidades exibidas na Landing Page: grupos (12), seleções (48), ranking FIFA e jogos da fase de grupos (72, todos com data **e horário**, estádio e cidade). Para cada jogo da fase de grupos, os valores de **data, horário, estádio e cidade** SHALL corresponder fielmente ao registrado na versão vigente de `copa2026_jogos_primeira_fase.txt`, identificando cada jogo pelo confronto (mandante × visitante); nenhuma sede, data ou horário SHALL ser trocado, inventado ou inferido. Quando o arquivo `copa2026_jogos_primeira_fase.txt` for **revisado**, o seed SHALL ser ressincronizado e o banco recriado para refletir a versão vigente da fonte. Os horários SHALL ser transcritos no fuso da fonte (Brasília, UTC-3), sem conversão. Os nomes de estádio e cidade SHALL ser transcritos conforme o texto de `/fontes` (ex.: `Estadio Akron`, `GEHA Field at Arrowhead`, `Nova Jersey`). A carga de jogadores e dos jogos do mata-mata permanece opcional/futura.

O ranking FIFA SHALL ser carregado apenas para as seleções presentes em `copa2026_ranking_fifa.txt` (cobertura parcial das 48 — as demais ficam sem registro de ranking, sem valores inventados). Os códigos de seleção (`Codigo`, padrão de 3 letras) e a `BandeiraUrl` SHALL usar os códigos oficiais de país (ISO 3166-1 alpha-3 / código FIFA) — identificadores factuais de país, não dados de torneio — já que `/fontes` não os fornece; o vínculo entre arquivos (ranking ↔ seleção ↔ jogo) SHALL ser feito pelos nomes canônicos de `copa2026_grupos.txt`.

#### Scenario: Seed em banco vazio
- **WHEN** a aplicação inicia com as tabelas de dados oficiais vazias
- **THEN** os dados de grupos, seleções, ranking FIFA, estádios/sedes e jogos da fase de grupos são inseridos a partir dos arquivos de `/fontes`, com cada jogo carregado com data, horário, estádio e cidade oficiais

#### Scenario: Seed não duplica dados
- **WHEN** a aplicação inicia novamente com os dados oficiais já presentes
- **THEN** o SeedData não insere registros duplicados

#### Scenario: Uso exclusivo dos dados oficiais
- **WHEN** os dados carregados são inspecionados
- **THEN** os 12 grupos, as 48 seleções, os 72 jogos da fase de grupos (com seus horários oficiais) e o ranking FIFA correspondem aos arquivos de `/fontes`, sem confrontos, grupos, seleções ou horários inventados

#### Scenario: Data, estádio e cidade fiéis à fonte
- **WHEN** um jogo da fase de grupos é comparado com `copa2026_jogos_primeira_fase.txt` pelo confronto (mandante × visitante)
- **THEN** sua data, horário, estádio e cidade são exatamente os registrados na fonte, sem sedes ou datas trocadas

#### Scenario: Ressincronização após revisão da fonte
- **WHEN** `copa2026_jogos_primeira_fase.txt` é revisado (alterando data, horário, estádio ou cidade de jogos) e o banco é recriado
- **THEN** o seed reflete a versão revisada da fonte, e todos os 72 jogos voltam a corresponder confronto a confronto à fonte vigente, sem divergências remanescentes

#### Scenario: Ranking parcial sem invenção
- **WHEN** uma seleção não consta em `copa2026_ranking_fifa.txt`
- **THEN** ela é carregada normalmente, porém sem registro de `RankingFifa`, e nenhum valor de ranking é inventado para ela

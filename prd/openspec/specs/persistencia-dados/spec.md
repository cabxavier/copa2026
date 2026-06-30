# persistencia-dados

## Purpose

Define o modelo de domínio da Copa do Mundo 2026, a camada de persistência com EF Core + SQLite (`AppDbContext`), a inicialização do banco de dados, a carga de dados oficiais via SeedData idempotente e a persistência durável das simulações dos usuários, isolada dos dados oficiais.
## Requirements
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

### Requirement: Persistência com EF Core e SQLite
O sistema SHALL utilizar EF Core com provedor SQLite e expor um `AppDbContext` contendo um `DbSet` para cada entidade do domínio.

#### Scenario: DbContext expõe as entidades
- **WHEN** o `AppDbContext` é inspecionado
- **THEN** existe um `DbSet` para `Grupo`, `Selecao`, `Jogador`, `Jogo`, `RankingFifa`, `Simulacao` e `SimulacaoJogo`

#### Scenario: Connection string SQLite
- **WHEN** a aplicação é configurada
- **THEN** a connection string do SQLite é lida de `appsettings.json` e usada para registrar o `AppDbContext`

### Requirement: Inicialização do banco de dados
O sistema SHALL garantir que o banco SQLite exista e esteja com o esquema atualizado na inicialização da aplicação, criando-o automaticamente quando ausente.

#### Scenario: Banco ausente na primeira execução
- **WHEN** a aplicação inicia e o arquivo SQLite ainda não existe
- **THEN** o banco é criado com o esquema correspondente às entidades antes de atender requisições

#### Scenario: Banco já existente
- **WHEN** a aplicação inicia e o banco já existe com o esquema atual
- **THEN** os dados existentes são preservados e nenhuma recriação destrutiva ocorre

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

### Requirement: Persistência de simulações entre execuções
O sistema SHALL persistir as simulações dos usuários e seus resultados (`Simulacao` e `SimulacaoJogo`) no SQLite, mantendo-os disponíveis após o encerramento da aplicação.

#### Scenario: Simulação sobrevive ao reinício
- **WHEN** uma `Simulacao` com seus `SimulacaoJogo` é salva e a aplicação é reiniciada
- **THEN** a simulação e seus resultados continuam disponíveis para consulta

#### Scenario: Dados oficiais não são alterados pela simulação
- **WHEN** uma simulação registra resultados de jogos
- **THEN** os resultados simulados são gravados em `SimulacaoJogo`, sem modificar os registros oficiais de `Jogo`

### Requirement: Persistência de resultados oficiais da fase de grupos

O sistema SHALL permitir que a aplicação registre e atualize os placares oficiais dos jogos da fase de grupos diretamente na entidade `Jogo` (`Jogo.GolsMandante` e `Jogo.GolsVisitante`), persistindo-os no SQLite. Ambos os campos permanecem anuláveis: um resultado oficial só é considerado completo quando os dois estão preenchidos; tornar o placar incompleto SHALL voltar os campos a nulo. A gravação de resultados oficiais NÃO SHALL exigir migração de esquema, pois os campos já existem na entidade `Jogo`. Os resultados oficiais SHALL permanecer a única fonte de verdade da classificação oficial e SHALL ser independentes dos resultados do Simulador (`SimulacaoJogo`).

#### Scenario: Resultado oficial gravado em Jogo
- **WHEN** a aplicação salva o placar oficial de um jogo com ambos os gols informados
- **THEN** `Jogo.GolsMandante` e `Jogo.GolsVisitante` são persistidos no SQLite e ficam disponíveis após o reinício da aplicação

#### Scenario: Resultado oficial removido
- **WHEN** um placar oficial é tornado incompleto
- **THEN** `Jogo.GolsMandante` e `Jogo.GolsVisitante` voltam a nulo e o jogo deixa de contar na classificação oficial

#### Scenario: Independência entre oficial e simulação
- **WHEN** um resultado oficial é gravado em `Jogo`
- **THEN** os registros de `SimulacaoJogo` permanecem inalterados, e vice-versa


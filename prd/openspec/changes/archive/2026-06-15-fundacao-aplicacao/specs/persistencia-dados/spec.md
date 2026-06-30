## ADDED Requirements

### Requirement: Modelo de domínio da Copa
O sistema SHALL definir as entidades de domínio `Grupo`, `Selecao`, `Jogador`, `Jogo`, `RankingFifa`, `Simulacao` e `SimulacaoJogo`, com seus atributos e relacionamentos representando a estrutura da Copa do Mundo 2026.

#### Scenario: Relacionamento grupo e seleções
- **WHEN** um `Grupo` é carregado
- **THEN** é possível navegar para as `Selecao` que pertencem a esse grupo

#### Scenario: Relacionamento seleção e jogadores
- **WHEN** uma `Selecao` é carregada
- **THEN** é possível navegar para os `Jogador` que compõem o seu elenco, com nome, posição, idade, gols marcados e participações em copas

#### Scenario: Relacionamento jogo, seleções e estádio
- **WHEN** um `Jogo` é carregado
- **THEN** ele referencia a seleção mandante, a seleção visitante, a data, o grupo e as informações de estádio

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
O sistema SHALL popular o banco com os dados oficiais iniciais da Copa (seleções, grupos, jogadores, jogos e ranking FIFA) por meio de SeedData, de forma idempotente.

#### Scenario: Seed em banco vazio
- **WHEN** a aplicação inicia com as tabelas de dados oficiais vazias
- **THEN** os dados de seleções, grupos, jogadores, jogos e ranking FIFA são inseridos

#### Scenario: Seed não duplica dados
- **WHEN** a aplicação inicia novamente com os dados oficiais já presentes
- **THEN** o SeedData não insere registros duplicados

### Requirement: Persistência de simulações entre execuções
O sistema SHALL persistir as simulações dos usuários e seus resultados (`Simulacao` e `SimulacaoJogo`) no SQLite, mantendo-os disponíveis após o encerramento da aplicação.

#### Scenario: Simulação sobrevive ao reinício
- **WHEN** uma `Simulacao` com seus `SimulacaoJogo` é salva e a aplicação é reiniciada
- **THEN** a simulação e seus resultados continuam disponíveis para consulta

#### Scenario: Dados oficiais não são alterados pela simulação
- **WHEN** uma simulação registra resultados de jogos
- **THEN** os resultados simulados são gravados em `SimulacaoJogo`, sem modificar os registros oficiais de `Jogo`

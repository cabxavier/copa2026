## 1. Solução e projeto Blazor Web App

- [x] 1.1 Criar a solução `.slnx` `PortalCopa26` em `src/PortalCopa26/PortalCopa26.slnx`
- [x] 1.2 Criar o projeto Blazor Web App (.NET 10) `PortalCopa26` em `src/PortalCopa26/PortalCopa26/` com render mode Interactive Server e adicioná-lo à solução
- [x] 1.3 Validar build (`dotnet build`) e execução (`dotnet run`) do template padrão
- [x] 1.4 Criar a estrutura de pastas `Models/`, `Data/`, `Data/Configurations/`, `Services/` (manter `Components/` e `Pages/` do template)

## 2. Dependências EF Core + SQLite

- [x] 2.1 Adicionar pacotes `Microsoft.EntityFrameworkCore.Sqlite` e `Microsoft.EntityFrameworkCore.Design`
- [x] 2.2 Configurar ferramentas EF Core (`dotnet ef`) e verificar disponibilidade
- [x] 2.3 Adicionar connection string SQLite (`Data Source=portalcopa26.db`) em `appsettings.json`

## 3. Modelo de domínio (Models/)

- [x] 3.1 Criar entidade `Grupo` (Id, Nome) com coleção de `Selecao`
- [x] 3.2 Criar entidade `Selecao` (Id, Nome, Código/ISO, URL da bandeira, FK `GrupoId`) com coleções de `Jogador` e ranking
- [x] 3.3 Criar entidade `Jogador` (Id, Nome, Posicao, Idade, GolsMarcados, ParticipacoesCopas, FK `SelecaoId`)
- [x] 3.4 Criar entidade `Jogo` (Id, Data, FKs seleção mandante/visitante, FK `GrupoId`, Estadio/Cidade, placar oficial opcional)
- [x] 3.5 Criar entidade `RankingFifa` (Id, FK `SelecaoId`, Pontuacao, Posicao)
- [x] 3.6 Criar entidade `Simulacao` (Id, Nome/Descricao, DataCriacao) com coleção de `SimulacaoJogo`
- [x] 3.7 Criar entidade `SimulacaoJogo` (Id, FK `SimulacaoId`, FK `JogoId`, GolsMandante, GolsVisitante)

## 4. DbContext e configurações (Data/)

- [x] 4.1 Criar `AppDbContext` com `DbSet` para todas as sete entidades
- [x] 4.2 Criar `IEntityTypeConfiguration<T>` (Fluent API) em `Data/Configurations/` definindo chaves, relacionamentos e restrições (incluindo FKs sem cascata destrutiva entre `Jogo` e `SimulacaoJogo`)
- [x] 4.3 Aplicar as configurações via `ApplyConfigurationsFromAssembly` em `OnModelCreating`

## 5. Injeção de dependência e inicialização (Program.cs)

- [x] 5.1 Registrar o `AppDbContext` via `AddDbContextFactory` com provedor SQLite usando a connection string
- [x] 5.2 Gerar a migration inicial `InitialCreate` (`dotnet ef migrations add`)
- [x] 5.3 Aplicar as migrations na inicialização da aplicação (criar banco se ausente, preservar se existente)

## 6. SeedData (Data/)

- [x] 6.1 Criar serviço `SeedData`/`DbInitializer` idempotente (verifica existência antes de inserir)
- [x] 6.2 Popular dados oficiais: `Grupo`, `Selecao` (com bandeiras), `Jogador`, `Jogo` e `RankingFifa`
- [x] 6.3 Invocar o SeedData na startup após aplicar migrations

## 7. Verificação

- [x] 7.1 Executar a aplicação e confirmar criação do arquivo SQLite com o esquema esperado
- [x] 7.2 Confirmar que os dados de seed foram inseridos e que reiniciar não os duplica
- [x] 7.3 Confirmar persistência: salvar uma `Simulacao` de teste, reiniciar e verificar que ela permanece
- [x] 7.4 Confirmar `dotnet build` sem erros/avisos relevantes

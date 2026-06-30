## Why

O projeto PortalCopa26 possui requisitos e decisões arquiteturais definidos (CLAUDE.md) e um protótipo de UI validado, mas ainda não existe uma base de código executável. Antes de implementar qualquer página (Landing Page, Jogos, Grupos, Seleções, Ranking FIFA, Simulador) é necessário estabelecer a fundação técnica: a solução Blazor Web App, a camada de persistência (EF Core + SQLite), o modelo de domínio e a carga de dados iniciais. Sem essa base, nenhuma funcionalidade pode ser construída de forma consistente.

## What Changes

- Criar a solução `PortalCopa26` no formato `.slnx` dentro de `src/PortalCopa26/`, com o projeto Blazor Web App (.NET 10) em projeto único.
- Definir a estrutura inicial de pastas que permita futura migração para arquitetura em camadas: `Models`, `Data`, `Services`, `Components`, `Pages`.
- Configurar EF Core com provedor SQLite e registrar o `DbContext` da aplicação via injeção de dependência nativa.
- Criar o `AppDbContext` com os `DbSet` das entidades iniciais.
- Criar as entidades de domínio: `Grupo`, `Selecao`, `Jogador`, `Jogo`, `RankingFifa`, `Simulacao` e `SimulacaoJogo`, incluindo seus relacionamentos.
- Configurar a carga de dados iniciais oficiais da Copa via SeedData (seleções, grupos, jogadores, jogos e ranking FIFA).
- Garantir a criação/migração do banco SQLite na inicialização e a persistência das simulações entre execuções.
- Preparar pontos de extensão (registro de serviços, JSInterop futuro para Chart.js) sem implementar as funcionalidades de tela.

## Capabilities

### New Capabilities
- `infraestrutura-aplicacao`: estrutura da solução Blazor Web App em projeto único, organização de pastas preparada para camadas e configuração de injeção de dependência.
- `persistencia-dados`: modelo de domínio (entidades e relacionamentos), `DbContext`, configuração de EF Core + SQLite, inicialização do banco e carga de dados via SeedData, incluindo persistência de simulações.

### Modified Capabilities
<!-- Nenhuma. Não existem specs prévias; esta é a fundação inicial do projeto. -->

## Impact

- **Código novo**: solução `src/PortalCopa26/PortalCopa26.slnx` e projeto `PortalCopa26` (Blazor Web App).
- **Dependências (pacotes NuGet)**: `Microsoft.EntityFrameworkCore.Sqlite` e ferramentas EF Core; Bootstrap 5 já incluso no template; Chart.js será integrado futuramente via JSInterop.
- **Persistência**: arquivo de banco SQLite local versionado por configuração; criação automática na inicialização.
- **Configuração**: `Program.cs` (registro do `DbContext`, serviços e SeedData), `appsettings.json` (connection string).
- **Fora do escopo**: páginas/UI (Landing, Jogos, Grupos, Seleções, Ranking, Simulador), gráficos Chart.js, integrações externas, autenticação e área administrativa.

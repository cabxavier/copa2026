## Context

O PortalCopa26 é um portal informativo da Copa do Mundo 2026 (consulta de jogos, grupos, seleções, ranking FIFA e simulação de resultados). O `CLAUDE.md` define a stack (.NET 10, Blazor Web App, EF Core, SQLite, Bootstrap 5, JSInterop, Chart.js), a arquitetura em projeto único com possibilidade de migração futura para camadas, e o escopo geral. Existe um protótipo de UI validado como referência visual.

Esta mudança entrega apenas a **fundação técnica**: solução, projeto, persistência, modelo de domínio e dados iniciais. Nenhuma página ou gráfico é implementado aqui. As decisões abaixo precisam ser fixadas antes da codificação porque definem o modelo de dados e os pontos de extensão sobre os quais todas as funcionalidades futuras serão construídas.

## Goals / Non-Goals

**Goals:**
- Solução `.slnx` `PortalCopa26` com projeto único Blazor Web App (.NET 10) em `src/PortalCopa26/`.
- Estrutura de pastas (`Models`, `Data`, `Services`, `Components`, `Pages`) que permita extrair camadas futuramente sem reescrita.
- EF Core + SQLite configurados com `AppDbContext` e DI nativa.
- Modelo de domínio completo: `Grupo`, `Selecao`, `Jogador`, `Jogo`, `RankingFifa`, `Simulacao`, `SimulacaoJogo` com relacionamentos.
- Criação automática do banco na inicialização e SeedData idempotente com dados oficiais.
- Persistência durável das simulações, isolada dos dados oficiais.

**Non-Goals:**
- Páginas/UI (Landing, Jogos, Grupos, Seleções, Ranking, Simulador) e componentes visuais.
- Integração do Chart.js / JSInterop (apenas preparar pontos de extensão).
- Lógica de negócio do simulador (cálculo de classificação).
- Integração com APIs externas, autenticação, autorização e área administrativa.

## Decisions

### Modo de renderização do Blazor Web App
Usar o template Blazor Web App com **render mode Interactive Server** como padrão inicial. Rationale: simplicidade de acesso ao `DbContext` no servidor sem expor a camada de dados ao cliente WebAssembly; alinhado ao foco informativo. Alternativa considerada: InteractiveAuto/WebAssembly — descartada por adicionar complexidade de serialização de dados e divisão de projeto que contraria o "projeto único" desta fundação.

### Ciclo de vida do DbContext
Registrar via `AddDbContextFactory<AppDbContext>` (ou `AddDbContext` com `ServiceLifetime.Scoped`), preferindo **`IDbContextFactory`**. Rationale: em Blazor Server, componentes podem viver mais que o escopo de uma requisição e o uso concorrente de um `DbContext` Scoped causa erros; a factory cria contextos de vida curta por operação. Alternativa: `AddDbContext` Scoped — adequado para serviços simples, mas arriscado em componentes interativos de longa duração.

### Idioma e nomenclatura
Entidades e propriedades de domínio em **português** (`Selecao`, `Jogo`, `RankingFifa`), seguindo a linguagem do domínio e do `CLAUDE.md`. Identificadores sem acento para evitar problemas de mapeamento (`Selecao`, não `Seleção`). Tabelas mapeadas com nomes explícitos quando necessário.

### Estratégia de esquema do banco
Para a fundação, usar `EnsureCreated()` via inicializador na startup **ou** Migrations do EF Core. Decisão: adotar **Migrations** desde o início (migration inicial `InitialCreate`). Rationale: o projeto evoluirá (novas entidades/campos) e migrations preservam dados; `EnsureCreated` não suporta evolução incremental. Trade-off: leve overhead inicial de configurar as ferramentas EF.

### Organização da persistência e SeedData
`AppDbContext` em `Data/`. Configurações de entidade via `IEntityTypeConfiguration<T>` (Fluent API) em `Data/Configurations/` para manter as entidades POCO limpas. SeedData implementado como serviço (`Data/SeedData` ou `DbInitializer`) chamado na startup, **idempotente** (verifica existência antes de inserir). Rationale: separa POCO de configuração (preparação para camadas) e evita duplicação ao reiniciar.

### Modelo de simulação
`Simulacao` representa uma sessão de simulação do usuário; `SimulacaoJogo` armazena o resultado simulado de cada jogo (placar) referenciando o `Jogo` oficial sem alterá-lo. A classificação gerada é derivável dos `SimulacaoJogo` (cálculo fica fora desta fundação). Rationale: mantém os dados oficiais imutáveis e permite múltiplas simulações independentes.

### Connection string e localização do arquivo
Connection string em `appsettings.json` (`"Data Source=portalcopa26.db"`), com o arquivo SQLite no diretório de execução. Rationale: configuração padrão e portável; ajustável por ambiente.

## Risks / Trade-offs

- **Uso concorrente do `DbContext` em Blazor Server** → Mitigação: usar `IDbContextFactory` e contextos de vida curta por operação.
- **SeedData não idempotente causaria duplicação a cada start** → Mitigação: verificar existência (ou checar `AnyAsync()`) antes de inserir; cobrir com cenário de spec.
- **Acoplamento prematuro de UI à camada de dados dificultaria migração para camadas** → Mitigação: acesso a dados sempre por serviços em `Services/`, nunca `DbContext` direto em componentes.
- **Dados oficiais iniciais incompletos/desatualizados** (sorteio/elencos) → Mitigação: SeedData centralizado e facilmente substituível; dados tratados como conteúdo, não regra.
- **Escolha de render mode pode mudar ao introduzir UI rica** → Mitigação: render mode é configurável; a fundação não depende de WebAssembly.

## Migration Plan

1. Criar solução `.slnx` e projeto Blazor Web App em `src/PortalCopa26/`.
2. Adicionar pacotes EF Core (Sqlite, Design/Tools).
3. Criar entidades em `Models/` e `AppDbContext` + configurações em `Data/`.
4. Configurar DI e connection string em `Program.cs`/`appsettings.json`.
5. Gerar migration inicial e aplicar na startup; executar SeedData idempotente.
6. Rollback: como é a base inicial sem dados de produção, basta remover o arquivo `.db` e/ou reverter a migration; nenhuma estratégia de dados legados é necessária.

## Open Questions

- Render mode definitivo (Server vs Auto) será reavaliado quando a UI interativa for implementada.
- Origem/atualização dos dados oficiais (sorteio dos grupos, elencos) — quais fontes congelar no SeedData inicial.
- Bandeiras das seleções: usar URLs do padrão da API pública da FIFA (campo de URL na `Selecao`) — confirmar formato.

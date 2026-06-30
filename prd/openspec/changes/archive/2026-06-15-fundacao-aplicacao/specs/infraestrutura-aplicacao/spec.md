## ADDED Requirements

### Requirement: Solução Blazor Web App em projeto único
O sistema SHALL ser estruturado como uma solução `.slnx` chamada `PortalCopa26`, contendo um único projeto Blazor Web App em .NET 10, localizada em `src/PortalCopa26/`.

#### Scenario: Compilação da solução
- **WHEN** a solução `src/PortalCopa26/PortalCopa26.slnx` é compilada com `dotnet build`
- **THEN** a compilação conclui sem erros e produz o projeto Blazor Web App executável

#### Scenario: Execução da aplicação
- **WHEN** a aplicação é iniciada com `dotnet run`
- **THEN** o servidor Blazor Web App sobe e a página inicial padrão responde via HTTP

### Requirement: Estrutura de pastas preparada para camadas
O projeto SHALL organizar o código em pastas `Models`, `Data`, `Services`, `Components` e `Pages`, de modo que o domínio, a persistência e os serviços possam ser extraídos para projetos separados no futuro sem reescrita.

#### Scenario: Separação de responsabilidades
- **WHEN** uma entidade de domínio é adicionada
- **THEN** ela reside em `Models` e não depende de tipos de UI (`Components`/`Pages`)

#### Scenario: Isolamento da persistência
- **WHEN** o acesso a dados é necessário
- **THEN** o `DbContext` e as configurações de EF Core residem em `Data` e são consumidos via serviços/injeção de dependência, não diretamente pelos componentes de UI

### Requirement: Injeção de dependência nativa configurada
O sistema SHALL registrar seus serviços e o `DbContext` no contêiner de injeção de dependência nativo do ASP.NET Core, em `Program.cs`, utilizando o ciclo de vida adequado para cada serviço.

#### Scenario: Resolução do DbContext
- **WHEN** um componente ou serviço solicita o `AppDbContext` via construtor
- **THEN** o contêiner de DI fornece uma instância configurada com o provedor SQLite

#### Scenario: Ponto de extensão para serviços futuros
- **WHEN** um novo serviço de aplicação precisa ser disponibilizado
- **THEN** ele pode ser registrado em `Program.cs` sem alterar a configuração existente do `DbContext`

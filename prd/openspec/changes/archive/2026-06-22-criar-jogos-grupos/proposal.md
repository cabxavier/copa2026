## Why

O PortalCopa26 já possui a Landing Page, mas ainda não oferece uma página dedicada para consultar os jogos do torneio. O menu de navegação atual (`NavMenu`) traz apenas os itens do template (Home, Counter, Weather) e não há ponto de acesso para a tabela completa de partidas da fase de grupos. Os usuários não conseguem explorar todos os jogos, filtrar por grupo ou ver detalhes de estádio. Esta change entrega a capacidade de consulta de jogos, usando exclusivamente os dados oficiais já carregados via Seed Data.

## What Changes

- Nova página `Jogos` (`Components/Pages/Jogos/`) com rota `/jogos`, render mode interativo (`InteractiveServer`) e acessível a partir do menu de navegação.
- Listagem de todos os jogos da fase de grupos **agrupados por data** e ordenados por data crescente (desempate estável por `Id`).
- Filtro por grupo (A a L) que recompõe a listagem mantendo o agrupamento por data.
- Exibição das informações de grupo e de estádio (estádio e cidade) de cada partida.
- Botão "Ver Grupos" como chamada de ação para a futura página de grupos.
- Novo serviço `JogosService` (camada `Services/`) que isola os componentes do EF Core e fornece os jogos e a lista de grupos via DTOs.
- Componentes Blazor reutilizáveis: `Jogos.razor`, `JogosFiltro.razor`, `JogosDataHeader.razor`, `JogoCard.razor`.

## Capabilities

### New Capabilities
- `jogos`: Consulta da tabela de jogos do torneio — listagem agrupada por data, ordenação por data/hora, filtro por grupo, exibição de informações de grupo e estádio, e acesso aos grupos.

### Modified Capabilities
<!-- Nenhuma capacidade existente tem seus requisitos alterados. -->

## Impact

- **Código novo**: `Components/Pages/Jogos/{Jogos,JogosFiltro,JogosDataHeader,JogoCard}.razor`, `Services/IJogosService.cs`, `Services/JogosService.cs`, `Services/Dtos/JogosDtos.cs`.
- **Código alterado**: registro do `JogosService` em `Program.cs`; inclusão de um item "Jogos" no `Components/Layout/NavMenu.razor` apontando para `/jogos`.
- **Dados**: somente leitura dos dados já semeados (`Jogos`, `Grupos`, `Selecoes`) — nenhum dado fictício é gerado e nenhuma migração de banco é necessária.
- **Dependências**: nenhuma nova dependência; reutiliza EF Core, Bootstrap 5 e os padrões de serviço/DTO já existentes.

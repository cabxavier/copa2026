## 1. Camada de dados (Serviço e DTOs)

- [x] 1.1 Criar `Services/Dtos/JogosDtos.cs` com `JogoListaDto` (Id, Data, GrupoNome, MandanteNome, MandanteBandeira, VisitanteNome, VisitanteBandeira, Estadio, Cidade) e `GrupoOpcaoDto` (Id, Nome)
- [x] 1.2 Criar `Services/IJogosService.cs` com `ObterJogosAsync(int? grupoId = null)` e `ObterGruposAsync()`
- [x] 1.3 Criar `Services/JogosService.cs` usando `IDbContextFactory<AppDbContext>`, `AsNoTracking`, `Include` de mandante/visitante/grupo, filtro opcional por `GrupoId` e ordenação `OrderBy(j => j.Data).ThenBy(j => j.Id)` (desempate estável, pois os dados não têm horário); `ObterGruposAsync` projeta `Grupo.Nome` em `GrupoOpcaoDto`
- [x] 1.4 Registrar `IJogosService`/`JogosService` em `Program.cs` (mesmo padrão do `LandingPageService`)

## 2. Componentes da página de Jogos

- [x] 2.1 Criar `Components/Pages/Jogos/JogoCard.razor` exibindo mandante e visitante (nome + bandeira), grupo e estádio (estádio + cidade), recebendo um `JogoListaDto` como parâmetro
- [x] 2.2 Criar `Components/Pages/Jogos/JogosGrupoHeader.razor` que renderiza o cabeçalho de uma seção (título do grupo + contagem de jogos)
- [x] 2.3 Criar `Components/Pages/Jogos/JogosFiltro.razor` com seleção de grupo (opção "Todos" + grupos via `GrupoOpcaoDto`, rótulo = `Grupo.Nome`, chips `chip orange` como o protótipo) emitindo `EventCallback<int?>`
- [x] 2.4 Criar `Components/Pages/Jogos/Jogos.razor` com `@page "/jogos"` e `@rendermode InteractiveServer`, injetando `IJogosService`, agrupando os jogos por `GrupoNome` (grupos A→L) na ordem retornada pelo serviço, compondo `JogosFiltro`, `JogosGrupoHeader` e `JogoCard`, e recarregando a lista ao mudar o filtro
- [x] 2.5 Adicionar o botão "Ver Grupos" na página `Jogos` em estado "em breve"/desabilitado (sem navegação, sem link quebrado, pois a página de grupos está fora de escopo)

## 3. Navegação

- [x] 3.1 Adicionar um item "Jogos" em `Components/Layout/NavMenu.razor` (`NavLink href="jogos"`) para a nova página
- [x] 3.2 Ajustar o menu do cabeçalho (`Components/Layout/MainLayout.razor`) para apontar "Jogos" a `/jogos` e marcar o item ativo conforme a rota atual (Home/Jogos)

## 4. Verificação

- [x] 4.1 Compilar a solução (`dotnet build`) sem erros
- [x] 4.2 Validar manualmente em `/jogos`: listagem agrupada por grupo (A→L) e ordenada, filtro por grupo reativo (render mode interativo), informações de grupo e estádio visíveis, botão "Ver Grupos" em estado "em breve", item "Jogos" do cabeçalho ativo conforme a rota
- [x] 4.3 Confirmar que nenhum componente `.razor` acessa `AppDbContext` diretamente e que nenhum dado fictício foi introduzido

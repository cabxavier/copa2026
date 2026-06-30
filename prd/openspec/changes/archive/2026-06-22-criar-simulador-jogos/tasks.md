## 1. Lógica de classificação (testável, sem banco)

- [x] 1.1 Criar `Services/Dtos/SimuladorDtos.cs` com `SimuladorJogoDto` (JogoId, MandanteId, MandanteNome, MandanteBandeira, VisitanteId, VisitanteNome, VisitanteBandeira, GolsMandante?, GolsVisitante?), `ClassificacaoLinhaDto` (Posicao, SelecaoId, Nome, Bandeira, J, V, E, D, GolsPro, GolsContra, Saldo, Pontos) e `SelecaoGrupoDto` (Nome, Bandeira, RankingPosicao?). **Reusar** `GrupoOpcaoDto` de `Services/Dtos/JogosDtos.cs` (já existe) — não redeclarar para evitar tipo duplicado no namespace `Services.Dtos`
- [x] 1.2 Criar `CalculadoraClassificacao` (helper puro) que recebe as 4 seleções do grupo e os placares informados e devolve as linhas ordenadas, contabilizando 3/1/0 pontos e somando J/V/E/D/GP/GC/SG; ignorar jogos sem ambos os placares
- [x] 1.3 Implementar a ordenação com critérios oficiais: pontos → saldo de gols → gols marcados → confronto direto. Para **2 empatadas**, decidir pelo resultado do confronto direto; para **3+ empatadas**, montar **mini-tabela** apenas com os jogos entre elas e reaplicar pontos → SG → gols marcados. Empate indecidível recai em ordem estável e determinística por `SelecaoId`
- [x] 1.4 Cobrir `CalculadoraClassificacao` com testes unitários dos 4 critérios (pontos, SG, gols, confronto direto 2-way e mini-tabela 3-way) e do fallback estável — *projeto `PortalCopa26.Tests` (xUnit, net10.0) adicionado à solução; 8 testes, todos aprovados (`dotnet test`)*

## 2. Serviço de simulação (persistência da simulação corrente)

- [x] 2.1 Criar `Services/ISimuladorService.cs` com: `ObterGruposAsync()`, `ObterJogosDoGrupoAsync(int grupoId)`, `ObterSelecoesDoGrupoAsync(int grupoId)`, `ObterPlacaresAsync()` (simulação corrente), `ObterClassificacaoAsync(int grupoId)`, `SalvarPlacarAsync(int jogoId, int? golsMandante, int? golsVisitante)`, `LimparGrupoAsync(int grupoId)`, `LimparTudoAsync()`
- [x] 2.2 Criar `Services/SimuladorService.cs` usando `IDbContextFactory<AppDbContext>`; helper privado `ObterOuCriarSimulacaoCorrenteAsync` que localiza/cria a única `Simulacao` pelo `Nome` sentinela fixo `"__corrente__"`, definindo `DataCriacao = DateTime.UtcNow` na criação (satisfaz `Nome` NOT NULL/`MaxLength(100)`) — sentinela encapsulado, nunca exposto à UI
- [x] 2.3 Implementar `SalvarPlacarAsync`: com ambos os placares, inserir/atualizar o `SimulacaoJogo (SimulacaoId, JogoId)`; se algum placar for nulo, remover a linha correspondente (respeitando o índice único)
- [x] 2.4 Implementar `LimparGrupoAsync` (remove `SimulacaoJogo` dos jogos do grupo) e `LimparTudoAsync` (remove todos os `SimulacaoJogo` da simulação corrente)
- [x] 2.5 Implementar leituras com `AsNoTracking`/projeções para DTOs; jogos do grupo via `GrupoId`, ordenados por `Data`/`Id`; seleções do grupo com bandeira e posição de ranking quando disponível
- [x] 2.6 Registrar `ISimuladorService`/`SimuladorService` na DI em `Program.cs` (mesmo padrão do `JogosService`)

## 3. Componentes da página de Simulador

- [x] 3.1 Criar `Components/Pages/Simulador/SimuladorJogo.razor` — uma linha de jogo (mandante nome+bandeira, dois inputs numéricos min=0, visitante nome+bandeira); validar/normalizar entrada (inteiro ≥ 0) e emitir `EventCallback` com (jogoId, golsMandante?, golsVisitante?)
- [x] 3.2 Criar `Components/Pages/Simulador/SimuladorGrupo.razor` — painel "GRUPO X — PLACARES" compondo os `SimuladorJogo` do grupo (6 jogos por grupo: round-robin de 4 seleções), repassando o callback de alteração
- [x] 3.3 Criar `Components/Pages/Simulador/ClassificacaoGrupo.razor` — tabela Bootstrap 5 responsiva (`table-responsive`) com colunas #, SELEÇÃO, J, V, E, D, SG, PTS alinhadas (numéricas centralizadas), destaque das 2 primeiras linhas (Classifica) e da 3ª (Wildcard) e legenda
- [x] 3.4 Criar `Components/Pages/Simulador/SimuladorResumo.razor` — lista "Seleções do Grupo X" (nome, bandeira, ranking) a partir de `SelecaoGrupoDto`

## 4. Página e navegação

- [x] 4.1 Criar `Components/Pages/Simulador/Simulador.razor` com `@page "/simulador"` e `@rendermode InteractiveServer`, injetando `ISimuladorService`; carregar grupos, placares da simulação corrente e o grupo A por padrão
- [x] 4.2 Implementar chips dos grupos A–L com destaque do ativo e troca de grupo (recarrega jogos, classificação e seleções do grupo selecionado)
- [x] 4.3 Ao alterar um placar: atualizar estado em memória, persistir o delta via `SalvarPlacarAsync` (auto-save, sem ação de salvar) e recalcular a classificação do grupo exibido
- [x] 4.4 Adicionar ações "Limpar Grupo" e "Limpar Tudo" chamando os métodos do serviço e atualizando a UI
- [x] 4.5 Garantir restauração: ao abrir/retornar à página, os placares da simulação corrente são carregados e refletidos nos inputs e na classificação
- [x] 4.6 Em `Components/Layout/MainLayout.razor` (menu real da aplicação), converter a âncora "Simulador" (`#simulador`) em link de rota `/simulador` e adicionar o estado ativo `IsSimulador` em `OnParametersSet`, espelhando o `IsJogos` existente (não usar `NavMenu.razor`, que é resíduo do template e não é referenciado)

## 5. Verificação

- [x] 5.1 Compilar a solução (`dotnet build`) sem erros
- [x] 5.2 Validar em `/simulador`: troca de grupos, entrada de placares recalculando em tempo real, pontos 3/1/0 e colunas J/V/E/D/SG/PTS corretas — *validado ao vivo (navegador headless via puppeteer-core/Chrome, circuito SignalR): digitar placares recalcula em tempo real (México J3 V3 SG+3 9 pts), troca de grupo exibe "GRUPO B — PLACARES". Também confirmado que placar parcial não computa. Corrigido bug "Grupo Grupo" (Grupo.Nome já contém "Grupo "): títulos/chips agora exibem "Grupo A", "GRUPO A — PLACARES" etc.*
- [x] 5.3 Validar desempate: empate em pontos resolvido por SG → gols → confronto direto; conferir o caso de **3+ empatadas** (mini-tabela entre elas) e o fallback de ordem estável por `SelecaoId` quando indecidível — *validado com cenário no Grupo A: México 9 pts (1º) e empate triplo (África/Coreia/Tcheca) a 3 pts/SG−1; ciclo indecidível na mini-tabela → ordem estável por `SelecaoId` (2,3,4), conforme renderizado*
- [x] 5.4 Validar destaques (top 2 = Classifica, 3º = Wildcard) e legenda; tabela responsiva e alinhada em viewport estreito — *legenda e classes `q1`/`q2` + `table-responsive` confirmadas no HTML renderizado*
- [x] 5.5 Validar persistência automática: editar placares, sair e retornar (inclusive reiniciando a app) e confirmar restauração; "Limpar Grupo"/"Limpar Tudo" persistem o estado zerado — *validado ao vivo (headless): auto-save ao digitar; após reload os 6 placares e a classificação são restaurados; "Limpar Grupo" zera inputs+classificação ao vivo e o estado zerado persiste após reload*
- [x] 5.6 Confirmar que nenhum componente `.razor` acessa `AppDbContext` diretamente e que nenhum dado fictício foi introduzido (mata-mata e 8 melhores terceiros permanecem fora) — *acesso a dados exclusivamente via `ISimuladorService`; classificação via helper puro; nenhum dado inventado*

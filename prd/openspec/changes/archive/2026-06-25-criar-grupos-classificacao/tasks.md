## 1. DTOs e serviço de Grupos

- [x] 1.1 Confirmar reúso de `ClassificacaoLinhaDto` e do record de jogo-com-placar (`SimuladorJogoDto`) e de `GrupoOpcaoDto`; adicionar comentário no record indicando que representa um jogo de grupo genérico (melhoria futura: renomear)
- [x] 1.2 Criar `Services/IGruposService.cs` com `ObterGruposAsync`, `ObterJogosDoGrupoAsync(int grupoId)`, `ObterClassificacaoAsync(int grupoId)` e `SalvarResultadoAsync(int jogoId, int? golsMandante, int? golsVisitante)`
- [x] 1.3 Criar `Services/GruposService.cs` usando `IDbContextFactory<AppDbContext>` e `AsNoTracking`, lendo os placares **oficiais** de `Jogo` (`GolsMandante`/`GolsVisitante`)
- [x] 1.4 Implementar `ObterClassificacaoAsync` delegando a `CalculadoraClassificacao.Calcular` sobre os jogos oficiais (sem duplicar lógica)
- [x] 1.5 Implementar `SalvarResultadoAsync`: normalizar negativos para 0; com ambos os gols, gravar em `Jogo`; com algum nulo, voltar ambos a nulo (remover resultado); `SaveChangesAsync`
- [x] 1.6 Registrar `IGruposService`/`GruposService` em `Program.cs` (`AddScoped`)

## 2. Componente de classificação compartilhado

- [x] 2.1 Mover `ClassificacaoGrupo.razor` de `Components/Pages/Simulador/` para `Components/Shared/` e adicionar `@using PortalCopa26.Components.Shared` ao `_Imports.razor`
- [x] 2.2 Adicionar parâmetro opcional `MostrarGolsProContra` (default `false`) que insere **duas colunas separadas GP e GC** entre as colunas D e SG, com classes `col-v`/`col-d` nos cabeçalhos/células de V e D
- [x] 2.3 Adicionar parâmetro opcional `MostrarLegenda` (default `true`) controlando a legenda interna do cabeçalho do painel
- [x] 2.4 Adicionar slot opcional `RenderFragment? Acoes` no `panel-head`, sem alterar o comportamento atual do Simulador
- [x] 2.5 Adicionar parâmetro opcional `MostrarEliminado` (default `false`) que marca como eliminadas (classe `q3`) as seleções a partir do 4º lugar
- [x] 2.6 Adicionar slot opcional `RenderFragment? Rodape` ao final do painel (abaixo da tabela), para a legenda completa da página Grupos
- [x] 2.7 Verificar que o Simulador continua renderizando a tabela como antes (com legenda interna, sem GP/GC, sem marcação de eliminado e sem ações/rodapé) — sem regressão visual

## 3. Página Grupos e jogos editáveis

- [x] 3.1 Criar `Components/Pages/Grupos/Grupos.razor` com `@page "/grupos"`, `@rendermode InteractiveServer`, `@inject IGruposService` e `<PageTitle>Portal Copa26 — Grupos</PageTitle>`, carregando grupo A por padrão
- [x] 3.2 Reproduzir a marcação do protótipo `../prototipo/grupos.html`: `section.page.grupos` → `div.page-head` (`h1` "Grupos" + `p.sub` + `button.btn-simular` "⚽ Simular") + `div.chips` + `div.two-col` com os dois `div.panel` (jogos à esquerda, classificação à direita)
- [x] 3.3 Implementar chips dos 12 grupos (A–L) com classe `.chip`, rótulo com nome completo ("Grupo X"), destaque do selecionado (`active`) e troca de grupo (atualiza títulos "Classificação — Grupo X" e "Jogos — Grupo X")
- [x] 3.4 Painel de classificação reusando `ClassificacaoGrupo` com `MostrarGolsProContra=true`, `MostrarEliminado=true`, `MostrarLegenda=false` e legenda de 3 itens (Classificado/Melhor Terceiro/Eliminado) no slot `Rodape`
- [x] 3.5 Criar `Components/Pages/Grupos/GrupoJogoLinha.razor` no formato `div.grow` do protótipo (`div.gt` mandante, `div.score-box` com dois `input.score-in` + `span.sep`, `div.gt.r` visitante, `button.btn-salvar`, `div.gdate`), placar na faixa `0..99`, **botão de salvar explícito** e confirmação "✓ salvo" temporária (`Task.Delay` + `CancellationTokenSource`/`IDisposable`)
- [x] 3.6 Painel de jogos (`div.glist`) listando os jogos do grupo (cronológico) com `GrupoJogoLinha`; ao acionar salvar, chamar `SalvarResultadoAsync` e recarregar classificação e jogos (recálculo imediato)
- [x] 3.7 Botão "Simular" do cabeçalho navega para `/simulador` via `NavigationManager`
- [x] 3.8 Habilitar a navegação para `/grupos` na navegação real da aplicação — o cabeçalho `Components/Layout/MainLayout.razor` (o `NavMenu.razor` do template não é referenciado/usado): trocar o `<span class="menu-soon">Grupos</span>` por link ativo (`IsGrupos`) e habilitar os botões "Ver Grupos" da página Jogos (`Jogos.razor`) e do painel da Landing Page (`LandingPage/SimuladorPainel.razor`)
- [x] 3.9 Adicionar bloco de CSS da página Grupos em `wwwroot/css/portal.css`, escopado em `.grupos` onde compartilha classes (chips, `panel-head h3`, cabeçalhos V/D, `q3`); estilos exclusivos (`.btn-simular`, `.score-box`, `.score-in`, `.btn-salvar`, `.saved-tag`, `.gdate`) globais — sem afetar Simulador/Jogos

## 4. Isolamento e validações

- [x] 4.1 Garantir que a classificação da página Grupos use exclusivamente `Jogo` (oficial) e nunca `SimulacaoJogo`
- [x] 4.2 Garantir que `SalvarResultadoAsync` não toca em `SimulacaoJogo`
- [x] 4.3 Validar normalização de inputs (faixa `0..99`) e remoção de resultado quando o placar fica incompleto
- [x] 4.4 Confirmar que nenhum resultado oficial é gravado sem a ação explícita de salvar (sem auto-save)

## 5. Verificação

- [x] 5.1 `dotnet build` sem erros/warnings novos
- [x] 5.2 Executar a aplicação e validar fidelidade ao protótipo: page-head com botão Simular, chips ("Grupo X"), troca de grupo, jogos à esquerda/classificação à direita, colunas (J/V/E/D/GP/GC/SG/PTS), destaque de classificados/melhor terceiro/eliminado, legenda única no rodapé e confirmação "✓ salvo"
- [x] 5.6 Validar regressão visual do Simulador após mover/estender `ClassificacaoGrupo` (tabela, legenda interna, sem GP/GC, sem eliminado, recálculo)
- [x] 5.3 Registrar um resultado oficial e confirmar recálculo imediato e persistência após reinício
- [x] 5.4 Confirmar que placares no Simulador não alteram a classificação oficial e vice-versa
- [x] 5.5 `openspec validate criar-grupos-classificacao --strict` sem erros

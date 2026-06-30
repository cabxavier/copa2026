## Context

O banco SQLite já contém os 12 grupos, 48 seleções e 72 jogos oficiais (`Jogo`), com `GolsMandante`/`GolsVisitante` anuláveis ainda não preenchidos. O Simulador (change anterior) introduziu duas peças reutilizáveis e relevantes:

- `Services/CalculadoraClassificacao.cs` — lógica **pura** (sem banco) que recebe jogos do grupo com placar e devolve `IReadOnlyList<ClassificacaoLinhaDto>` já ordenada pelos critérios oficiais (pontos → saldo → gols marcados → confronto direto, com fallback estável por `SelecaoId`). A ordem coincide com `fontes/copa2026_regras_negocio.txt`.
- `Components/Pages/Simulador/ClassificacaoGrupo.razor` — tabela de classificação com destaque de classificados (1º/2º) e wildcard (3º) e legenda. Hoje exibe colunas `# SELEÇÃO J V E D SG PTS` (sem GP/GC, sem botão Simular).

O `ClassificacaoLinhaDto` já carrega `GolsPro` e `GolsContra`, embora a tabela atual não os mostre. O Simulador persiste placares em `SimulacaoJogo` (independente de `Jogo`).

A página Grupos precisa do mesmo cálculo, porém sobre os **resultados oficiais** de `Jogo`, e deve permitir **editar** esses resultados. A diretriz explícita do pedido é evitar duplicação da lógica de classificação.

## Goals / Non-Goals

**Goals:**
- Página `/grupos` interativa, fiel ao protótipo, com chips dos 12 grupos, classificação oficial, jogos do grupo e botão Simular.
- Classificação oficial calculada dinamicamente a partir de `Jogo`, com colunas adicionais GP e GC.
- Registro/edição de resultados oficiais persistidos em `Jogo`, com recálculo imediato.
- Reúso máximo de `CalculadoraClassificacao` e `ClassificacaoGrupo.razor`.
- Isolamento total entre classificação oficial (`Jogo`) e Simulador (`SimulacaoJogo`).

**Non-Goals:**
- Cálculo dos 8 melhores terceiros entre grupos e qualquer etapa de mata-mata.
- Critérios de desempate por fair play/cartões (não há dados de cartões no modelo) e por sorteio — recaem no fallback estável existente.
- Alterações no Simulador ou em seu serviço.
- Qualquer migração de esquema (os campos de placar de `Jogo` já existem).

## Decisions

### Decisão 1: Reusar `CalculadoraClassificacao` como lógica única de classificação
O cálculo permanece centralizado em `CalculadoraClassificacao.Calcular(...)`. O novo `GruposService` apenas alimenta o cálculo com os placares **oficiais** de `Jogo`, enquanto o `SimuladorService` continua alimentando com `SimulacaoJogo`. Isso satisfaz a diretriz de não duplicar a lógica.

- **Entrada do cálculo**: o `Calcular` recebe hoje `IEnumerable<SimuladorJogoDto>`. O record `SimuladorJogoDto` é, na prática, "um jogo do grupo com placar opcional" — genérico apesar do nome. Para a primeira versão, o `GruposService` **reutiliza esse record** ao montar os jogos oficiais, evitando criar um DTO paralelo e mantendo a assinatura do cálculo intacta. (Renomear o record para um nome neutro, ex.: `JogoPlacarDto`, fica registrado como melhoria futura — fora do escopo desta change para manter o diff reduzido e não tocar no Simulador.)
- **Alternativa considerada**: duplicar o cálculo dentro de `GruposService` — rejeitada (duplicação proibida pela diretriz).
- **Alternativa considerada**: extrair uma interface/DTO neutro agora — adia-se para não inflar o escopo.

### Decisão 2: Novo `IGruposService`/`GruposService` sobre `IDbContextFactory`
Segue o padrão de `JogosService`/`SimuladorService` (contextos de vida curta via `IDbContextFactory<AppDbContext>`, `AsNoTracking` em leituras, retorno por DTOs; nenhum acesso a `DbContext` na UI). Métodos:

- `ObterGruposAsync()` → `IReadOnlyList<GrupoOpcaoDto>` (chips A–L).
- `ObterJogosDoGrupoAsync(int grupoId)` → jogos do grupo com placar **oficial** (`Jogo.GolsMandante/GolsVisitante`), ordenados por data/Id. Reutiliza o record de jogo-com-placar.
- `ObterClassificacaoAsync(int grupoId)` → `IReadOnlyList<ClassificacaoLinhaDto>` chamando `CalculadoraClassificacao.Calcular` sobre os jogos oficiais.
- `SalvarResultadoAsync(int jogoId, int? golsMandante, int? golsVisitante)` → grava em `Jogo` (rastreado): com ambos os gols, define os placares; se algum for nulo, volta ambos a nulo (resultado removido). Normaliza negativos para 0.

`GrupoOpcaoDto` é reutilizado de `JogosDtos`. Registro do serviço em `Program.cs` (`AddScoped`), como os demais.

### Decisão 3: Promover `ClassificacaoGrupo` a componente compartilhado e parametrizar legenda/colunas
Para reusar a tabela sem acoplar a capacidade Grupos à pasta do Simulador e sem conflito de legenda:
- **Mover** `ClassificacaoGrupo.razor` de `Components/Pages/Simulador/` para uma pasta compartilhada `Components/Shared/`, ajustando a referência no Simulador (a página `Simulador.razor` deixa de tê-lo no mesmo namespace, então adiciona-se `@using PortalCopa26.Components.Shared` no `_Imports.razor`, que passa a valer para todas as páginas, inclusive Grupos). Isso respeita o princípio do `CLAUDE.md` de componentes próprios por capacidade e evita o acoplamento Grupos → Simulador.
  - **Alternativa considerada**: manter o componente na pasta do Simulador e adicionar `@using PortalCopa26.Components.Pages.Simulador` em Grupos — rejeitada por acoplar capacidades.
- **Parâmetros opcionais** (todos com default que preserva o comportamento atual do Simulador):
  - `MostrarGolsProContra` (bool, default `false`): quando `true`, insere **duas colunas separadas GP e GC** entre D e SG, como no protótipo `grupos.html`. (Coloração dos cabeçalhos V/D fica por conta do CSS escopado em `.grupos`.)
  - `MostrarEliminado` (bool, default `false`): quando `true`, marca como eliminadas (classe `q3`) as seleções a partir do 4º lugar. **Para a página Grupos será `true`**; o Simulador mantém o default.
  - `MostrarLegenda` (bool, default `true`): a legenda interna do cabeçalho do painel. **Para a página Grupos será `false`** — a legenda completa é renderizada no rodapé do painel via slot `Rodape` (ver abaixo), evitando a legenda duplicada e o conflito de texto com a do Simulador ("Classifica"/"Wildcard").
  - `RenderFragment? Acoes`: slot no `panel-head` para ações (não utilizado pela página Grupos, que coloca o botão Simular no cabeçalho da página; mantido para uso futuro).
  - `RenderFragment? Rodape`: slot ao final do painel, abaixo da tabela. **A página Grupos o usa** para a legenda de 3 itens ("Classificado (1º e 2º de cada grupo)", "Melhor Terceiro (8 melhores avançam)", "Eliminado"). O Simulador não o usa.
- **Alternativa considerada**: criar `ClassificacaoOficial.razor` separado — rejeitada por duplicar markup/estilos da tabela.

### Decisão 4: Edição de placar oficial com salvamento explícito (componente próprio)
Criar `Components/Pages/Grupos/Grupos.razor` (página, `@page "/grupos"`, `@rendermode InteractiveServer`) que orquestra: chips dos 12 grupos (mesmo padrão visual do Simulador, classe `.chip`), painel de jogos com edição (à esquerda) e painel de classificação (`ClassificacaoGrupo` compartilhado, à direita). Cada linha de jogo é o componente `GrupoJogoLinha.razor`, com dois inputs numéricos `score-in` (faixa `0..99`) **e um botão de salvar explícito**.

- **Salvamento explícito (e não auto-save):** decisão de UX deliberada. Diferente do Simulador — que salva a cada tecla em uma simulação descartável —, o resultado **oficial** exige uma confirmação consciente. Por isso `SimuladorJogo.razor` **não** é reutilizado: `GrupoJogoLinha.razor` é um componente próprio. Ao confirmar, a página chama `SalvarResultadoAsync` e recarrega a classificação e os jogos do grupo (recálculo imediato), e o jogo exibe uma confirmação visual "✓ salvo" por ~1,8s.
  - **Alternativa considerada**: auto-save igual ao Simulador — rejeitada para evitar gravações oficiais acidentais a cada dígito.
  - **Robustez:** a confirmação "✓ salvo" usa `Task.Delay` com `CancellationTokenSource` e `IDisposable`, evitando `StateHasChanged` após o descarte do componente (ex.: navegação para fora de `/grupos` durante a espera).

### Decisão 5: Fidelidade visual ao protótipo com CSS próprio da página
A marcação da página SHALL reproduzir a estrutura do protótipo `../prototipo/grupos.html`: `section.page.grupos` → `div.page-head` (`h1` + `p.sub` + `button.btn-simular`) → `div.chips` → `div.two-col` com dois `div.panel` (jogos à esquerda, classificação à direita). O painel de jogos usa `div.glist > div.grow` contendo `div.gt`, `div.score-box` (dois `input.score-in` + `span.sep`), `div.gt.r`, `button.btn-salvar` e `div.gdate`.

O protótipo revisado introduziu elementos sem equivalente em `portal.css` (cabeçalho da página, botão Simular, caixa de placar com inputs, botão Salvar, tag "✓ salvo", colunas V/D coloridas, marcação de eliminado). Portanto, **adiciona-se um bloco de CSS próprio da página** em `wwwroot/css/portal.css`, **escopado em `.grupos`** onde houver risco de afetar outras telas que compartilham classes (`.chip`, `.panel-head h3`, cabeçalhos de tabela). Estilos com nomes exclusivos (`.btn-simular`, `.score-in`, `.btn-salvar`, `.saved-tag`, `.score-box`, `.gdate`) são globais. O Simulador, que reusa `ClassificacaoGrupo`, permanece inalterado por não estar sob `.grupos` nem usar `MostrarGolsProContra`/`MostrarEliminado`.

### Decisão 6: Navegação e menu
A navegação principal da aplicação é o cabeçalho de `Components/Layout/MainLayout.razor` (layout padrão em `Routes.razor`); o `NavMenu.razor` do template Blazor **não é referenciado** e é código morto. Portanto, habilitar "Grupos" no `MainLayout` (substituindo o placeholder `menu-soon`/"Em breve" por um link ativo, com estado `IsGrupos`) e habilitar os botões "Ver Grupos" já existentes na página Jogos e no painel do Simulador da Landing Page (que estavam `disabled` por a página ainda não existir). O botão "Simular" da página Grupos fica no **cabeçalho da página** (`button.btn-simular` com rótulo "⚽ Simular") e usa `NavigationManager` para ir a `/simulador`. A página SHALL incluir `<PageTitle>` ("Portal Copa26 — Grupos"), como as demais páginas.

## Risks / Trade-offs

- **Mover `ClassificacaoGrupo` para `Components/Shared/` pode quebrar a referência atual do Simulador** → Mitigação: ajustar o `@using` em `_Imports.razor` (passa a valer para todas as páginas) e validar visualmente o Simulador após a mudança (regressão coberta pela task 2.x e 5.x). Os parâmetros novos têm default que preserva o comportamento atual.
- **Nome do DTO genérico (`SimuladorJogoDto`) reutilizado fora do Simulador pode confundir** → Mitigação: documentar no código que o record representa um jogo-de-grupo-com-placar; registrar a renomeação como melhoria futura.
- **Divergência de UX entre Grupos (salvar explícito) e Simulador (auto-save)** → Mitigação: decisão consciente e documentada (Decisão 4); o contexto difere (resultado oficial vs simulação descartável), o que justifica a diferença.
- **Critérios de desempate 5–6 (fair play/cartões, sorteio) não modelados** → Mitigação: o fallback determinístico por `SelecaoId` já existente cobre o caso; documentado como limitação conhecida alinhada ao Simulador e à observação de `fontes` ("primeira versão poderá implementar apenas os primeiros critérios").
- **Edição concorrente de resultados (UI interativa)** → Mitigação: contextos de vida curta por operação e releitura após salvar evitam estado obsoleto; carga por grupo é pequena (6 jogos).
- **Confusão entre resultado oficial e simulado pelo usuário** → Mitigação: rótulos claros ("resultado oficial") e separação física das telas/serviços; nenhum cruzamento de dados entre `Jogo` e `SimulacaoJogo`.
- **Divergência de ordem dos critérios entre o pedido e `fontes`** → o pedido lista "confronto direto" como 2º critério; `fontes/copa2026_regras_negocio.txt` o define como 4º. Seguimos `fontes` (fonte oficial declarada no CLAUDE.md), que é exatamente o que `CalculadoraClassificacao` já implementa.

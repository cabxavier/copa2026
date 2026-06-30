## Context

A capacidade Simulador completa o núcleo do PortalCopa26. O protótipo `../prototipo/simulador.html` define a UX-alvo: chips de grupos, painel de placares (6 jogos por grupo: round-robin de 4 seleções, 3 por seleção), tabela de classificação recalculada em tempo real e lista de seleções do grupo. O modelo de dados já existe (`Simulacao`, `SimulacaoJogo`) com configuração EF Core (FK para `Jogo` sem cascata e índice único `(SimulacaoId, JogoId)`). Os jogos da fase de grupos (72) já estão semeados com `GrupoId`, mandante, visitante, estádio, cidade e horário. Convenção a observar: `Grupo.Nome` já contém o prefixo (ex.: `"Grupo A"`), portanto a UI não deve prefixar "Grupo" novamente.

Restrições do projeto: nenhum componente `.razor` acessa `AppDbContext`; acesso a dados via serviço usando `IDbContextFactory<AppDbContext>`; Bootstrap 5 como base visual; sem dados fictícios; render mode Interactive Server.

## Goals / Non-Goals

**Goals:**
- Página `/simulador` interativa espelhando o protótipo, com 4 componentes reutilizáveis (`SimuladorGrupo`, `SimuladorJogo`, `ClassificacaoGrupo`, `SimuladorResumo`).
- Lógica de classificação e desempate isolada em código testável (pontos → SG → gols → confronto direto), reutilizando os jogos oficiais já semeados.
- Persistência automática de uma única simulação corrente, sem nome, com auto-save por edição e restauração ao retornar.
- Tabela Bootstrap responsiva com colunas alinhadas, destaque de classificados (top 2) e wildcard (3º).

**Non-Goals:**
- 8 melhores terceiros entre grupos e qualquer etapa de mata-mata.
- Múltiplas simulações nomeadas, histórico ou comparação de cenários.
- Critérios de desempate 5 (fair play) e 6 (sorteio), pois o simulador não modela cartões; o empate residual cai em ordem estável determinística.

## Decisions

### Simulação corrente única (singleton lógico)
Manter **uma** linha `Simulacao` como "corrente". O serviço obtém-ou-cria essa simulação localizando-a pelo `Nome` sentinela fixo **`"__corrente__"`** (única linha com esse nome); na criação, define `Nome = "__corrente__"` e `DataCriacao = DateTime.UtcNow`. A configuração EF Core exige `Nome` (NOT NULL, `MaxLength(100)`) — o sentinela satisfaz a restrição. O sentinela é encapsulado no serviço e **nunca vaza para a UI**. `Simulacao.Nome` deixa de ser pedido ao usuário. Alternativa descartada: nova entidade/flag `EhCorrente` — desnecessária para o escopo de uma simulação só; reusar a tabela existente evita migration.

### Persistência por placar via SimulacaoJogo
Cada jogo com **ambos** os placares preenchidos vira um `SimulacaoJogo (SimulacaoId, JogoId, GolsMandante, GolsVisitante)`. Limpar um placar remove a linha correspondente. Auto-save é disparado no evento de alteração do input (debounce simples por edição), gravando/atualizando/removendo apenas o `SimulacaoJogo` afetado — escopo mínimo de I/O. O índice único `(SimulacaoId, JogoId)` garante no máximo um resultado por jogo.

### Modelo de estado na UI
A página carrega, ao inicializar, todos os `SimulacaoJogo` da simulação corrente num dicionário `JogoId → (golsMandante?, golsVisitante?)`. Os componentes operam sobre DTOs; ao editar, a página atualiza o estado em memória, persiste o delta e recalcula a classificação do grupo exibido. Recalcular é barato (4 times, 6 jogos por grupo), então o cálculo roda no servidor a cada edição.

### Serviço e DTOs
`ISimuladorService`/`SimuladorService` expõem: obter grupos; obter jogos do grupo; obter a simulação corrente (placares); salvar/limpar placar de um jogo; limpar grupo; limpar tudo. A classificação é calculada por um helper puro (`CalculadoraClassificacao`) que recebe jogos + placares e devolve linhas ordenadas — testável sem banco. DTOs em `Services/Dtos/SimuladorDtos.cs`: `SimuladorJogoDto`, `ClassificacaoLinhaDto`, `GrupoOpcaoDto` (reusar o existente se aplicável), `SelecaoGrupoDto`.

### Desempate por confronto direto (inclui 3+ empatados)
Após ordenar por (pontos, SG, gols), as seleções remanescentes empatadas são tratadas por confronto direto:
- **Duas empatadas**: compara o resultado do confronto direto entre elas (quando há placar).
- **Três ou mais empatadas**: monta uma **mini-tabela (sub-grupo)** apenas com os jogos entre as empatadas e reaplica pontos → SG → gols marcados dentro desse sub-grupo (regra oficial FIFA — não é comparação par a par).
Sem decisão possível (placares insuficientes entre as empatadas), mantém ordem estável por `SelecaoId` para determinismo. A `CalculadoraClassificacao` recebe os jogos do grupo e resolve isso internamente, permitindo teste sem banco.

### Componentização (alinha aos componentes obrigatórios)
- `SimuladorJogo.razor`: uma linha de jogo (mandante/bandeira, inputs, visitante/bandeira), emite `EventCallback` de placar.
- `SimuladorGrupo.razor`: painel de placares do grupo (compõe os 3 `SimuladorJogo`).
- `ClassificacaoGrupo.razor`: tabela Bootstrap responsiva com destaque de classificados/wildcard e legenda.
- `SimuladorResumo.razor`: lista de seleções do grupo (nome, bandeira, ranking) — equivalente ao bloco "Seleções do Grupo" do protótipo.

### Navegação no menu real (`MainLayout.razor`, não `NavMenu.razor`)
A navegação efetiva da aplicação é o `<nav class="menu">` em `Components/Layout/MainLayout.razor`, onde "Simulador" **já existe como âncora `#simulador`**. A integração SHALL converter essa âncora em **link de rota `/simulador`** e adicionar o estado ativo `IsSimulador` em `OnParametersSet` (espelhando o `IsJogos` existente). O arquivo `Components/Layout/NavMenu.razor` é resíduo do template padrão (contém `Counter`/`Weather`), **não é referenciado** por nenhum componente e **não deve ser usado** como ponto de integração.

## Risks / Trade-offs

- **Gravações frequentes a cada tecla** → debounce por edição e persistência só do `SimulacaoJogo` alterado; volume de I/O baixo (máx. 72 linhas).
- **Concorrência de abas sobre a simulação única** → fora do escopo (uso single-user local); última escrita vence.
- **Desempate incompleto (sem fair play/sorteio)** → documentado como Non-Goal; ordem estável evita não-determinismo visual.
- **Reuso de `Simulacao.Nome` com valor sentinela** → encapsular a obtenção da simulação corrente no serviço para que o sentinela não vaze para a UI.

## Migration Plan

- Sem migration de esquema: as tabelas `Simulacao`/`SimulacaoJogo` já existem. Não é necessário recriar o `.db`.
- Implementar serviço + componentes, registrar na DI e adicionar a rota/nav. Rollback = remover a página, componentes, serviço e o item de navegação; dados da simulação corrente permanecem inertes nas tabelas.

## Open Questions

- Nenhuma bloqueante. (Comportamento de empate residual e ausência de fair play já decididos como ordem estável.)

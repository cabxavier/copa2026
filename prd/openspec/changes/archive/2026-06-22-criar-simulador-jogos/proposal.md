## Why

A capacidade **Simulador** é uma das funcionalidades principais do PortalCopa26 e ainda não foi implementada — a Landing Page já oferece um call-to-action para ela. Os usuários precisam poder informar placares hipotéticos da fase de grupos e ver, em tempo real, a classificação recalculada segundo os critérios oficiais de desempate da FIFA, com a simulação preservada automaticamente entre visitas e execuções da aplicação.

## What Changes

- Nova página `/simulador` (`@rendermode InteractiveServer`) baseada no protótipo `../prototipo/simulador.html`.
- Seletor de grupos (A–L) por chips, exibindo, para o grupo ativo: os 6 jogos do grupo (round-robin de 4 seleções; 3 jogos por seleção) com entrada de placares, a classificação simulada e a lista de seleções do grupo.
- Cálculo automático da classificação a cada alteração de placar, aplicando os **critérios oficiais de desempate dentro do grupo** (pontos → saldo de gols → gols marcados → confronto direto), com pontos por vitória/empate/derrota (3/1/0).
- Destaque visual dos **2 primeiros classificados** de cada grupo e do **3º colocado (candidato a wildcard)**, com legenda.
- Ações **Limpar Grupo** e **Limpar Tudo**.
- Persistência automática de uma **única simulação corrente** em SQLite (sem exigir nome): cada alteração de placar é salva automaticamente e a simulação é restaurada ao retornar à página.
- Novos componentes reutilizáveis: `SimuladorGrupo.razor`, `SimuladorJogo.razor`, `ClassificacaoGrupo.razor`, `SimuladorResumo.razor`.
- Novo serviço `SimuladorService` (sem acesso a `DbContext` em componentes) e DTOs próprios; lógica de classificação/desempate isolada e testável.
- Tabela Bootstrap 5 responsiva, com colunas alinhadas e leitura clara dos dados.

Fora do escopo: classificação dos **8 melhores terceiros** entre grupos e qualquer etapa de **mata-mata**.

## Capabilities

### New Capabilities
- `simulador`: Simulação da fase de grupos — entrada de placares por jogo, cálculo de classificação com critérios oficiais de desempate, destaque de classificados/wildcard e persistência automática da simulação corrente.

### Modified Capabilities
<!-- Nenhuma capacidade existente tem seus requisitos alterados. -->

## Impact

- **Código novo**: `Components/Pages/Simulador/` (página + 4 componentes), `Services/ISimuladorService.cs` + `SimuladorService.cs`, `Services/Dtos/SimuladorDtos.cs`, e lógica de classificação/desempate em `Services/`.
- **Modelos existentes**: reutiliza `Simulacao` e `SimulacaoJogo`. `Simulacao.Nome` deixa de ser exigido do usuário — a simulação corrente usa o sentinela interno `"__corrente__"` (satisfaz a restrição NOT NULL existente).
- **DI**: registro do `SimuladorService` em `Program.cs`.
- **Navegação**: no menu real (`MainLayout.razor`), converter a âncora "Simulador" (`#simulador`) em rota `/simulador` com estado ativo `IsSimulador` (espelhando `IsJogos`). `NavMenu.razor` é resíduo do template e não é usado.
- **Persistência**: gravações na simulação corrente a cada edição de placar (`persistencia-dados`).
- Sem novas dependências externas; sem alterações de banco além do uso das tabelas já existentes.

## Context

A capacidade Grupos já grava o placar oficial das partidas na entidade `Jogo` (`GolsMandante`/`GolsVisitante`) via `GruposService.SalvarResultadoAsync`. As páginas Jogos e Landing Page, porém, projetavam DTOs sem esses campos e renderizavam status/placar fixos. O objetivo é exibir, nessas duas páginas, o resultado oficial persistido, mantendo a arquitetura existente (serviços + DTOs, sem EF Core nos componentes) e sem tocar no schema do banco.

Esta é uma change retroativa: o código já foi implementado e compila, e as specs principais de `jogos` e `landing-page` já foram sincronizadas. O design registra as decisões tomadas durante a implementação.

## Goals / Non-Goals

**Goals:**
- Exibir em Jogos (`JogoCard`) e Home (`ProximosJogos`) o placar oficial persistido na tabela `Jogo`.
- Diferenciar visualmente partida encerrada ("ENCERRADO" + placar) de partida sem resultado ("AGENDADO"/"PRÓXIMO" + "— : —").
- Reaproveitar a persistência já existente, sem mudança de schema.

**Non-Goals:**
- Atualização ao vivo entre abas/usuários (a leitura ocorre por carga de página).
- Alterar o Simulador, que opera sobre `SimulacaoJogo` (simulações do usuário), independente do resultado oficial.
- Permitir edição do placar nas páginas Jogos/Home (a edição permanece exclusiva de Grupos).

## Decisions

- **Fonte única do placar = tabela `Jogo`.** Jogos e Home leem `GolsMandante`/`GolsVisitante` da entidade `Jogo`, a mesma escrita por Grupos. Alternativa descartada: derivar de `SimulacaoJogo` — rejeitada porque misturaria simulações do usuário com o resultado oficial, violando a separação definida no CLAUDE.md.
- **Campos nos DTOs + propriedade `TemResultado`.** `JogoListaDto` e `JogoResumoDto` recebem `int? GolsMandante` e `int? GolsVisitante`, com `TemResultado => GolsMandante is not null && GolsVisitante is not null`. Centraliza a regra "há resultado" em um único ponto, evitando duplicação na view. Alternativa descartada: calcular a condição direto no `.razor` — rejeitada por espalhar a regra.
- **Renderização condicional no componente.** `JogoCard` e `ProximosJogos` decidem o rótulo de status e o placar a partir de `TemResultado`. Mantém a lógica de apresentação na view e os serviços apenas como projeção de dados.
- **Leitura por carga de página.** Sem SignalR/observabilidade adicional: o placar reflete o último valor persistido a cada navegação/recarga, condizente com o render mode `InteractiveServer` já usado.

## Risks / Trade-offs

- [Sem atualização ao vivo: marcar resultado em Grupos com a aba Jogos/Home já aberta não atualiza automaticamente] → o valor aparece ao recarregar/reabrir a página; aceitável para um portal informativo. Evolução futura possível via notificação entre componentes.
- [Construtores posicionais dos records ganharam novos parâmetros] → mitigado: as únicas instanciações estão nos próprios serviços (`JogosService`, `LandingPageService`), ambos atualizados; build da solução validado sem erros.
- [Divergência momentânea entre código e specs por ser change retroativa] → mitigado: specs principais já sincronizadas e deltas desta change refletem exatamente o comportamento implementado.

## Migration Plan

Não há migração de dados nem de schema. As colunas `GolsMandante`/`GolsVisitante` já existem. Rollback = reverter os arquivos de código listados no Impact do proposal; nenhum estado persistido é afetado.

## Open Questions

Nenhuma. O escopo e o comportamento estão definidos e implementados.

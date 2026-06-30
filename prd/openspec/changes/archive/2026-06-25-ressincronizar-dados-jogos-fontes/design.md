## Context

A change `corrigir-dados-jogos-fontes` (arquivada) já alinhou o seed à fonte e a spec de `persistencia-dados` já exige fidelidade de data/horário/estádio/cidade. Posteriormente, `./fontes/copa2026_jogos_primeira_fase.txt` foi **revisado**, e a transcrição em `DadosCopa.Jogos` ficou para trás. Esta change é uma **ressincronização** pontual da transcrição, sem alterar arquitetura, esquema ou serviços.

## Goals / Non-Goals

**Goals:**
- `DadosCopa.Jogos` refletindo exatamente a versão vigente da fonte (data, hora, estádio, cidade) para os 72 jogos.
- Banco recriado e verificado contra a fonte (contagens e amostra).
- Spec reforçada com cenário de ressincronização.

**Non-Goals:**
- Qualquer mudança de esquema, serviço, DTO ou componente.
- Conversão de fuso horário (os horários permanecem como na fonte, Brasília/UTC-3).
- Alteração de confrontos, grupos, seleções ou ranking.

## Decisions

### Decisão 1: Manter o horário da fonte sem conversão
`DbInitializer` grava `Jogo.Data` combinando data + hora e rotulando como `DateTimeKind.Utc`, **sem** converter o relógio. A fonte declara os horários no fuso de Brasília; a aplicação os exibe tal como registrados. Manter esse comportamento preserva a consistência com a fonte e evita regressão na exibição (página Jogos e Grupos).

### Decisão 2: Recriar o banco em vez de migrar dados
O seed é idempotente (só insere em banco vazio). Para refletir a fonte revisada, o caminho é **apagar o `.db` local e re-semear** na inicialização — não há migração de esquema, pois apenas valores de seed mudaram.

### Decisão 3: Verificação contra a fonte
Após o re-seed, conferir contagens (12 grupos, 48 seleções, 72 jogos, ranking) e amostra de jogos (data/hora/estádio/cidade) diretamente no banco, comparando com `/fontes`.

## Risks / Trade-offs

- **Nova revisão futura da fonte volta a defasar o seed** → Mitigação: cenário explícito de ressincronização na spec e nota de processo (tratar revisão de `/fontes` como gatilho para re-seed).
- **Bancos `.db` de desenvolvedores precisam ser recriados** → Mitigação: seed idempotente recria automaticamente quando o arquivo é removido; documentado no Impact.

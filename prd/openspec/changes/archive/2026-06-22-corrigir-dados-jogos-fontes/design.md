## Context

O seed `DadosCopa.Jogos` (72 jogos da fase de grupos) foi transcrito com data, estádio e cidade incorretos: ao comparar cada confronto (mandante × visitante) com `./fontes/copa2026_jogos_primeira_fase.txt`, encontram-se **12 datas**, **60 estádios** e **61 cidades** divergentes. Os confrontos e grupos estão corretos; os horários foram corrigidos na change `exibir-horario-jogos`. A coluna `Jogo.Data` já é `DateTime` e o `DbInitializer` já combina data + hora — não há mudança de esquema nem de inicialização. O `SeedData` é idempotente (só insere com tabelas vazias), então a aplicação dos dados corrigidos exige recriar o banco local.

`/fontes` é a fonte de verdade designada pelo CLAUDE.md ("utilizar exclusivamente os dados da pasta ./fontes").

## Goals / Non-Goals

**Goals:**
- Corrigir `Data`, `Estadio` e `Cidade` dos 72 `JogoSeed` para refletir fielmente `/fontes`, casando por confronto (mandante × visitante).
- Preservar `Mandante`, `Visitante`, `Grupo` e `Hora` (já corretos).
- Garantir, por verificação, que nenhum jogo permaneça divergente da fonte.

**Non-Goals:**
- Alterar confrontos, grupos ou horários.
- Mudar esquema, `DbInitializer`, serviços, DTOs ou componentes.
- Reconciliar nomes de seleções/grupos (fora de escopo; já corretos).
- Migração de dados de produção (apenas re-seed local em desenvolvimento).

## Decisions

**Decisão: `/fontes` como fonte de verdade, transcrição verbatim.**
Cada `JogoSeed` recebe `Data`/`Estadio`/`Cidade` exatamente como em `copa2026_jogos_primeira_fase.txt`. Inclui as grafias da fonte: `Estadio Akron`/`Estadio BBVA` (sem acento), `Estádio Azteca` (com acento, como na fonte), `GEHA Field at Arrowhead`, e cidade `Nova Jersey`. Alternativa considerada: normalizar acentos/nomes "oficiais" (ex.: `Nova York/Nova Jersey`) — descartada para não introduzir dados fora de `/fontes` e manter rastreabilidade direta com a fonte.

**Decisão: casar por confronto (mandante × visitante), não por posição.**
A ordem das linhas do seed difere da ordem de `/fontes` em alguns grupos (ex.: Grupo D). A correção é aplicada casando cada jogo pelo par (mandante, visitante), que é único na fase de grupos. Alternativa: corrigir por posição — descartada por risco de atribuir a sede errada onde a ordem difere.

**Decisão: re-seed via recriação do banco local, sem migração.**
Como o esquema não muda e o seed é idempotente, aplica-se apagando o arquivo SQLite local e deixando o `DbInitializer` re-semear. Passo operacional, não código.

**Decisão: verificação automatizada confronto a confronto.**
Após o re-seed, comparar programaticamente cada jogo do banco com `/fontes` (data, estádio, cidade) por (mandante × visitante), exigindo **zero divergências**. Reaproveita o método de diagnóstico que revelou o problema.

## Risks / Trade-offs

- **Volume de edições (72 linhas) → risco de erro de transcrição** → Mitigação: gerar/conferir os valores diretamente de `/fontes` por confronto e validar com diff automatizado exigindo zero divergências antes de concluir.
- **Banco já semeado não recebe a correção automaticamente** (seed idempotente) → Mitigação: documentar e executar a recriação do `.db` local; em desenvolvimento não há perda (tabelas oficiais não têm dados de usuário).
- **Grafias de `/fontes` diferem do "oficial"** (ex.: `Nova Jersey`, `Estadio` sem acento) → Trade-off aceito: fidelidade à fonte designada prevalece sobre normalização estética; ajuste de grafia, se desejado, seria uma change futura sobre `/fontes`.
- **Dependência de `exibir-horario-jogos`** → esta change parte do seed já com `Hora`. Deve ser aplicada após `exibir-horario-jogos` para não conflitar na mesma região do arquivo.

## Why

O seed dos 72 jogos da fase de grupos (`DadosCopa.Jogos`) contém dados de **data, estádio e cidade incorretos** em relação à fonte oficial `./fontes/copa2026_jogos_primeira_fase.txt`. Uma comparação confronto a confronto (por mandante × visitante) revelou **12 datas**, **60 estádios** e **61 cidades** divergentes de 72 jogos — ou seja, quase todas as sedes foram atribuídas de forma trocada. Os confrontos (mandante/visitante) e os grupos estão corretos. Como o CLAUDE.md determina usar **exclusivamente** os dados de `/fontes` (sem inventar ou trocar sedes/confrontos), esses valores precisam ser corrigidos para que a página de Jogos exiba a sede e a data reais de cada partida.

## What Changes

- Corrigir, em `DadosCopa.Jogos`, os campos **Data**, **Estadio** e **Cidade** dos 72 jogos para refletir fielmente `./fontes/copa2026_jogos_primeira_fase.txt`, mapeando cada jogo pelo confronto (mandante × visitante).
- Os campos **Mandante**, **Visitante** e **Grupo** NÃO mudam (já estão corretos).
- Os **horários** (`Hora`), corrigidos na change `exibir-horario-jogos`, são preservados.
- Normalização: adotar o texto de `/fontes` como fonte de verdade para nomes de estádio (ex.: `Estadio Akron`, `GEHA Field at Arrowhead`) e cidade (ex.: `Nova Jersey`).
- Recriação do banco SQLite local para re-semear com os dados corrigidos (seed idempotente; sem mudança de esquema).

## Capabilities

### New Capabilities
<!-- Nenhuma capacidade nova é introduzida. -->

### Modified Capabilities
- `persistencia-dados`: a carga de dados oficiais via SeedData passa a refletir fielmente data, estádio e cidade de cada jogo conforme `/fontes`, corrigindo as atribuições de sede/data incorretas.

## Impact

- **Código alterado**: `Data/Seed/DadosCopa.cs` — correção dos argumentos `Data`, `Estadio` e `Cidade` nas 72 linhas de `JogoSeed(...)`.
- **Dados**: o banco SQLite local precisa ser recriado/re-semeado (apagar o arquivo `.db`); nenhuma migração de esquema é necessária.
- **Sem impacto** em serviços, DTOs ou componentes — `JogosService` e `JogoCard` apenas exibem os valores corrigidos.
- **Fonte oficial**: todos os valores transcritos exclusivamente de `./fontes/copa2026_jogos_primeira_fase.txt`.

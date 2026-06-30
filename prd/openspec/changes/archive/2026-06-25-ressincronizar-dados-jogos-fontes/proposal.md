## Why

O arquivo oficial `./fontes/copa2026_jogos_primeira_fase.txt` foi **revisado** após a change `corrigir-dados-jogos-fontes`, passando a registrar **datas, horários, estádios e cidades** diferentes dos que estavam no seed. Como a transcrição em `Data/Seed/DadosCopa.Jogos` não foi reacompanhada, o seed (e o banco dele derivado) ficou **defasado** em relação à fonte: os confrontos (mandante × visitante) e os grupos continuavam corretos, mas data, horário e sede de vários jogos divergiam do arquivo oficial.

Uma comparação confronto a confronto contra a fonte revisada confirmou as divergências (ex.: Grupo C — Brasil × Marrocos passou de 13/06 16h00 para 13/06 19h00; Grupo A jogo 3 passou de Estadio Akron/Guadalajara para Mercedes-Benz Stadium/Atlanta). O CLAUDE.md determina usar **exclusivamente** os dados de `/fontes`, então o seed precisa ser **ressincronizado** com a fonte revisada.

## What Changes

- Ressincronizar, em `DadosCopa.Jogos`, os campos **Data**, **Hora**, **Estadio** e **Cidade** dos 72 jogos para refletir fielmente a versão revisada de `./fontes/copa2026_jogos_primeira_fase.txt`, casando cada jogo pelo confronto (mandante × visitante).
- Os campos **Mandante**, **Visitante** e **Grupo** NÃO mudam (já estão corretos).
- Os **horários** são mantidos no fuso de Brasília (UTC-3), exatamente como na fonte, sem conversão (preserva o comportamento de exibição já existente).
- Recriar o banco SQLite local para re-semear com os dados ressincronizados (seed idempotente; sem mudança de esquema).
- Reforçar a spec de `persistencia-dados` com um cenário explícito de **ressincronização após revisão da fonte**, evitando que futuras revisões de `/fontes` passem despercebidas.

## Capabilities

### Modified Capabilities
- `persistencia-dados`: a carga de dados oficiais via SeedData passa a exigir, também, que quando `copa2026_jogos_primeira_fase.txt` for revisado, o seed seja ressincronizado e o banco recriado, mantendo data, horário, estádio e cidade fiéis à versão vigente da fonte.

## Impact

- **Código alterado**: `Data/Seed/DadosCopa.cs` — atualização de `Data`, `Hora`, `Estadio` e `Cidade` nas 72 linhas de `JogoSeed(...)`.
- **Dados**: o banco SQLite local foi recriado/re-semeado (arquivo `.db` apagado e regerado pelo `DbInitializer`); nenhuma migração de esquema.
- **Sem impacto** em serviços, DTOs ou componentes — `JogosService`, a página Grupos e o `JogoCard` apenas exibem os valores ressincronizados.
- **Fonte oficial**: todos os valores transcritos exclusivamente da versão vigente de `./fontes/copa2026_jogos_primeira_fase.txt`.

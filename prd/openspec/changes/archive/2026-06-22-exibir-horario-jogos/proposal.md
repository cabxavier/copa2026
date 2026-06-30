## Why

Os jogos da fase de grupos têm horário oficial definido em `./fontes/copa2026_jogos_primeira_fase.txt` (ex.: `16h00`, `14h00`), mas o seed atual (`JogoSeed`/`DbInitializer`) carrega apenas a data (`"2026-06-11"`), persistindo todos os jogos à meia-noite (00:00). Por isso a página de Jogos não consegue exibir o horário das partidas — qualquer tentativa de mostrar a hora resultaria em "00:00" para todos os 72 jogos. Esta change carrega o horário oficial e o exibe na label de cada partida (ex.: "16:00 Grupo A").

## What Changes

- **BREAKING (dados)**: o campo de data dos jogos passa a incluir o horário oficial. O `JogoSeed` ganha o horário (extraído de `/fontes`, formato `HH:mm`) e o `DbInitializer` combina data + hora ao persistir `Jogo.Data`. Como os 80 jogos já semeados têm data à meia-noite, o banco SQLite local precisa ser recriado/re-semeado para refletir os horários.
- Ordenação dos jogos passa a usar o horário real: `OrderBy(Data)` torna-se cronologicamente significativo dentro do mesmo dia (o desempate por `Id` deixa de ser a única garantia de ordem intra-dia).
- A label de cada partida (`JogoCard`) passa a exibir o horário no formato `HH:mm` junto ao nome do grupo (ex.: "16:00 Grupo A").
- Sem novas dependências, sem nova entidade e sem alteração de esquema (a coluna `Jogo.Data` já é `DateTime`; muda apenas o valor semeado).

## Capabilities

### New Capabilities
<!-- Nenhuma capacidade nova é introduzida. -->

### Modified Capabilities
- `persistencia-dados`: a carga de dados oficiais via SeedData passa a incluir o **horário** dos jogos da fase de grupos (data + hora a partir de `/fontes`), e o relacionamento `Jogo` passa a expor data **com horário**.
- `jogos`: cada partida exibida (`JogoCard`) passa a apresentar o **horário** da partida na label, e a ordenação intra-dia passa a refletir o horário real.

> Dependência: a capacidade `jogos` foi definida pela change `criar-jogos-grupos`, que ainda **não foi arquivada** em `openspec/specs/`. Esta change deve ser sincronizada/arquivada **após** `criar-jogos-grupos`, para que o delta de `jogos` se aplique sobre a spec já consolidada.

## Impact

- **Código alterado**:
  - `Data/Seed/DadosCopa.cs` — `record JogoSeed` ganha campo de horário; as 72 linhas de `JogoSeed(...)` recebem o horário oficial de `/fontes`.
  - `Data/DbInitializer.cs` — combinar data + hora ao montar `Jogo.Data` (parse de `HH:mm` somado à data, preservando `DateTimeKind.Utc`).
  - `Services/JogosService.cs` — ordenação `OrderBy(j => j.Data).ThenBy(j => j.Id)` mantida (agora cronológica de fato; `Id` segue como desempate estável).
  - `Components/Pages/Jogos/JogoCard.razor` — exibir `@Jogo.Data.ToString("HH:mm")` antes de `@Jogo.GrupoNome` na label.
- **Dados**: o banco SQLite local precisa ser recriado/re-semeado (apagar o arquivo `.db` ou aplicar re-seed) para que os horários apareçam; nenhuma migração de esquema é necessária.
- **Fonte oficial**: horários extraídos exclusivamente de `./fontes/copa2026_jogos_primeira_fase.txt` — nenhum horário fictício.

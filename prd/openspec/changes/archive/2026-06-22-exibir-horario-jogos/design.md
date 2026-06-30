## Context

A página de Jogos (change `criar-jogos-grupos`, já implementada) lista as partidas da fase de grupos, mas **não exibe horário**. A causa raiz está na carga de dados: `DadosCopa.JogoSeed` guarda apenas a data como string (`"2026-06-11"`) e `DbInitializer` faz `DateTime.SpecifyKind(DateTime.Parse(j.Data), DateTimeKind.Utc)`, persistindo `Jogo.Data` sempre à meia-noite. O arquivo oficial `./fontes/copa2026_jogos_primeira_fase.txt` contém os horários no formato `HHhMM` (ex.: `16h00`, `20h30`), mas eles são descartados no seed.

A coluna `Jogo.Data` já é `DateTime` — não há mudança de esquema. O SeedData é idempotente (só insere quando as tabelas estão vazias), então o banco SQLite local precisa ser recriado para refletir os horários.

## Goals / Non-Goals

**Goals:**
- Carregar o horário oficial de cada jogo da fase de grupos a partir de `/fontes`.
- Persistir `Jogo.Data` com data + hora (preservando `DateTimeKind.Utc`).
- Exibir o horário (`HH:mm`) na label de cada partida em `JogoCard`, junto ao grupo (ex.: "16:00 Grupo A").
- Manter a ordenação cronológica correta dentro de cada grupo.

**Non-Goals:**
- Fuso horário/conversão local: o horário é exibido como semeado (UTC), sem conversão para fuso da cidade-sede.
- Horários do mata-mata (não semeados).
- Alteração de esquema/migração EF Core (a coluna já é `DateTime`).
- Mudança no layout do `JogoCard` além de prefixar o horário na label de grupo.

## Decisions

**Decisão: adicionar um campo `Hora` ao `record JogoSeed`, em vez de mudar a string de data.**
`JogoSeed(string Data, string Hora, string Mandante, …)` com `Hora` no formato `"HHhMM"` (igual ao arquivo de `/fontes`, ex.: `"16h00"`). Mantém as datas existentes intactas e deixa o horário explícito e rastreável linha a linha. Alternativa considerada: fundir tudo em uma única string `"2026-06-11 16:00"` — descartada por exigir reescrever todas as 72 linhas e dificultar a conferência contra `/fontes`.

**Decisão: combinar data + hora no `DbInitializer`, preservando UTC.**
No `Select(j => new Jogo { … })`, montar `Data = DateTime.SpecifyKind(DateTime.Parse(j.Data).Add(ParseHora(j.Hora)), DateTimeKind.Utc)`, onde `ParseHora` converte `"16h00"` → `TimeSpan` (`TimeSpan.ParseExact`/split por `h`). Mantém o `DateTimeKind.Utc` já usado hoje, evitando ambiguidade do SQLite com datas. Alternativa: usar `DateTime.ParseExact` sobre `"yyyy-MM-dd HHhmm"` concatenado — equivalente; a soma de `TimeSpan` foi escolhida por isolar o parse do horário em um helper testável.

**Decisão: ordenação do serviço permanece `OrderBy(j => j.Data).ThenBy(j => j.Id)`.**
Com o horário real em `Data`, `OrderBy(Data)` passa a ordenar cronologicamente dentro do dia automaticamente; `ThenBy(Id)` continua como desempate estável para jogos com data e hora idênticas (existem pares no mesmo horário, ex.: dois jogos `20h00`). Nenhuma mudança de código no `JogosService` é necessária — apenas a semântica melhora.

**Decisão: exibir o horário na label de grupo do `JogoCard`.**
`<span class="grp-tag">@Jogo.Data.ToString("HH:mm") @Jogo.GrupoNome</span>`, reutilizando o `DateTime Data` já presente em `JogoListaDto` — sem alteração no DTO nem no serviço. Alternativa: novo campo formatado no DTO — descartada por ser desnecessária (a formatação na view é trivial e local).

**Decisão: re-seed via recriação do banco local, sem migração.**
Como o esquema não muda e o seed é idempotente, a forma de aplicar os horários é apagar o arquivo SQLite local (ou a base de desenvolvimento) e deixar o `DbInitializer` recriar e re-semear na próxima execução. Documentado como passo operacional, não como código.

## Risks / Trade-offs

- **Banco já semeado não recebe os horários automaticamente** (seed idempotente) → Mitigação: documentar o passo de recriação do `.db` local na verificação; em desenvolvimento o custo é nulo (dados oficiais, sem dados de usuário nas tabelas oficiais).
- **Horário em UTC vs. horário local da sede** → A exibição mostra o valor semeado (UTC) tal como em `/fontes`, que já traz o horário local de cada sede; não há conversão. Risco de interpretação dupla é baixo pois reproduz fielmente a fonte oficial. Conversão de fuso fica como melhoria futura.
- **Formato inesperado em alguma linha de `/fontes`** (ex.: `20h30`) → `ParseHora` deve tratar minutos diferentes de `00`; já há `20h30` nos dados. Mitigação: parse genérico `HHhMM`, não hard-code de `h00`.
- **Dependência da change `criar-jogos-grupos` não arquivada** → o delta de `jogos` aqui assume os requisitos definidos lá. Mitigação: sincronizar/arquivar `criar-jogos-grupos` antes de arquivar esta change.

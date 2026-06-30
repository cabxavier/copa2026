## 1. Seed: carregar o horário oficial

- [x] 1.1 Adicionar o campo `Hora` ao `record JogoSeed` em `Data/Seed/DadosCopa.cs` (`JogoSeed(string Data, string Hora, string Mandante, string Visitante, string Grupo, string Estadio, string Cidade)`)
- [x] 1.2 Preencher o `Hora` (formato `"HHhMM"`) em cada uma das 72 linhas de `JogoSeed(...)`, extraindo o horário exclusivamente de `./fontes/copa2026_jogos_primeira_fase.txt` (sem inventar horários; atenção a casos como `20h30`)

## 2. Inicialização: combinar data + hora

- [x] 2.1 Em `Data/DbInitializer.cs`, criar um helper para converter `"HHhMM"` em `TimeSpan` (parse genérico de horas e minutos)
- [x] 2.2 Ajustar a montagem do `Jogo` para `Data = DateTime.SpecifyKind(DateTime.Parse(j.Data).Add(<hora>), DateTimeKind.Utc)`, preservando `DateTimeKind.Utc`

## 3. Exibição na página de Jogos

- [x] 3.1 Em `Components/Pages/Jogos/JogoCard.razor`, exibir o horário na label de grupo: `<span class="grp-tag">@Jogo.Data.ToString("HH:mm") @Jogo.GrupoNome</span>`
- [x] 3.2 Confirmar que `JogosService` mantém `OrderBy(j => j.Data).ThenBy(j => j.Id)` (ordenação agora cronológica; `Id` como desempate estável) — nenhuma alteração de DTO necessária

## 4. Re-seed e verificação

- [x] 4.1 Recriar o banco SQLite local (apagar o arquivo `.db` de desenvolvimento) para que o `DbInitializer` re-semeie os jogos com horário
- [x] 4.2 Compilar a solução (`dotnet build`) sem erros
- [x] 4.3 Validar em `/jogos`: cada partida exibe o horário no formato `HH:mm` junto ao grupo (ex.: "16:00 Grupo A"), nenhum jogo exibe "00:00", e a ordem dentro de cada grupo está cronológica
- [x] 4.4 Conferir uma amostra de horários exibidos contra `./fontes/copa2026_jogos_primeira_fase.txt` (ex.: abertura México x África do Sul = 16:00)

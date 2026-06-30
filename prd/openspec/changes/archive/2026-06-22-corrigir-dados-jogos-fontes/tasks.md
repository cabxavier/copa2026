## 1. Correção dos dados no seed

- [x] 1.1 Para cada um dos 72 `JogoSeed` em `Data/Seed/DadosCopa.cs`, corrigir `Data`, `Estadio` e `Cidade` conforme `./fontes/copa2026_jogos_primeira_fase.txt`, casando pelo confronto (mandante × visitante); preservar `Mandante`, `Visitante`, `Grupo` e `Hora`
- [x] 1.2 Usar as grafias de `/fontes` verbatim (ex.: `Estadio Akron`, `Estadio BBVA`, `GEHA Field at Arrowhead`, cidade `Nova Jersey`)

## 2. Re-seed e verificação

- [x] 2.1 Compilar a solução (`dotnet build`) sem erros
- [x] 2.2 Recriar o banco SQLite local (apagar o arquivo `.db`) para re-semear com os dados corrigidos
- [x] 2.3 Verificar, por comparação automatizada confronto a confronto (mandante × visitante), que data, estádio e cidade de todos os 72 jogos correspondem a `/fontes` com **zero divergências**
- [x] 2.4 Validar em `/jogos` que cada partida exibe a sede e a data corretas (amostra conferida contra `/fontes`)

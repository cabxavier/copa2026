## 1. Ressincronização dos dados no seed

- [x] 1.1 Para cada um dos 72 `JogoSeed` em `Data/Seed/DadosCopa.cs`, atualizar `Data`, `Hora`, `Estadio` e `Cidade` conforme a versão vigente de `./fontes/copa2026_jogos_primeira_fase.txt`, casando pelo confronto (mandante × visitante); preservar `Mandante`, `Visitante` e `Grupo`
- [x] 1.2 Usar as grafias de `/fontes` verbatim (ex.: `Estadio Akron`, `Estadio BBVA`, `GEHA Field at Arrowhead`, cidade `Nova Jersey`) e os horários no fuso de Brasília sem conversão

## 2. Re-seed e verificação

- [x] 2.1 Compilar a solução (`dotnet build`) sem erros/warnings
- [x] 2.2 Recriar o banco SQLite local (apagar o arquivo `.db`) para re-semear com os dados ressincronizados
- [x] 2.3 Verificar contagens no banco recriado: 12 grupos, 48 seleções, 72 jogos, ranking carregado
- [x] 2.4 Verificar, por amostra, que data, horário, estádio e cidade dos jogos correspondem a `/fontes` (ex.: Grupo C conferido confronto a confronto)
- [x] 2.5 Rodar os testes (`dotnet test`) — validação de contagens do seed sem falhas

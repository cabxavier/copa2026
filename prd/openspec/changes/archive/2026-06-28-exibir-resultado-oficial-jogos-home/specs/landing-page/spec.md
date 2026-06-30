## MODIFIED Requirements

### Requirement: Próximos jogos
O componente `ProximosJogos` SHALL listar os próximos jogos ordenados por data, exibindo as seleções, o grupo e o estádio de cada partida. Cada partida SHALL refletir o placar oficial persistido na entidade `Jogo` (`GolsMandante`/`GolsVisitante`), gravado pela página Grupos: quando há resultado (ambos os gols preenchidos), a partida SHALL exibir o status "ENCERRADO" e o placar real (ex.: "2 : 1"); quando não há resultado, SHALL exibir o status "PRÓXIMO" e o placar vazio ("— : —"). A fonte do placar é exclusivamente a tabela `Jogo` (não o Simulador), lida via serviço a cada carregamento da página.

#### Scenario: Listagem ordenada por data
- **WHEN** a Landing Page é carregada
- **THEN** o `ProximosJogos` exibe uma lista de jogos ordenada por data crescente, com seleções, grupo e estádio

#### Scenario: Apenas jogos futuros priorizados
- **WHEN** existem jogos com data futura em relação à data atual
- **THEN** o componente prioriza a exibição dos próximos jogos a ocorrer

#### Scenario: Partida com resultado oficial
- **WHEN** um jogo listado tem placar oficial persistido (ambos os gols preenchidos)
- **THEN** ele é exibido com status "ENCERRADO" e o placar persistido (`GolsMandante` : `GolsVisitante`)

#### Scenario: Partida sem resultado oficial
- **WHEN** um jogo listado não tem placar oficial (um ou ambos os gols nulos)
- **THEN** ele é exibido com status "PRÓXIMO" e o placar vazio "— : —"

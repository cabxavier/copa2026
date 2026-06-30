## ADDED Requirements

### Requirement: Exibição do resultado oficial persistido
Cada partida exibida por `JogoCard` SHALL refletir o placar oficial persistido na entidade `Jogo` (`GolsMandante`/`GolsVisitante`), gravado pela página Grupos. Considera-se que há resultado quando ambos os gols estão preenchidos (não nulos). Quando há resultado, o `JogoCard` SHALL exibir o status "ENCERRADO" e o placar real (ex.: "2 : 1"); quando não há resultado, SHALL exibir o status "AGENDADO" e o placar vazio ("— : —"). A página de jogos NÃO SHALL consultar resultados do Simulador (`SimulacaoJogo`); a fonte do placar é exclusivamente a tabela `Jogo`. O placar é lido a cada carregamento da página, refletindo o último valor persistido.

#### Scenario: Partida com resultado oficial
- **WHEN** um `JogoCard` é renderizado para um jogo cujo placar oficial foi gravado (ambos os gols preenchidos)
- **THEN** ele exibe o status "ENCERRADO" e o placar persistido (`GolsMandante` : `GolsVisitante`)

#### Scenario: Partida sem resultado oficial
- **WHEN** um `JogoCard` é renderizado para um jogo sem placar oficial (um ou ambos os gols nulos)
- **THEN** ele exibe o status "AGENDADO" e o placar vazio "— : —"

#### Scenario: Reflexo do resultado gravado em Grupos
- **WHEN** um resultado é salvo na página Grupos e a página de jogos é carregada (ou recarregada) em seguida
- **THEN** o placar exibido na listagem de jogos corresponde ao valor persistido na tabela `Jogo`

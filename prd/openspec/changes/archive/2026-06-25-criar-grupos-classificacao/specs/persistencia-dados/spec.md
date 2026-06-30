## ADDED Requirements

### Requirement: Persistência de resultados oficiais da fase de grupos

O sistema SHALL permitir que a aplicação registre e atualize os placares oficiais dos jogos da fase de grupos diretamente na entidade `Jogo` (`Jogo.GolsMandante` e `Jogo.GolsVisitante`), persistindo-os no SQLite. Ambos os campos permanecem anuláveis: um resultado oficial só é considerado completo quando os dois estão preenchidos; tornar o placar incompleto SHALL voltar os campos a nulo. A gravação de resultados oficiais NÃO SHALL exigir migração de esquema, pois os campos já existem na entidade `Jogo`. Os resultados oficiais SHALL permanecer a única fonte de verdade da classificação oficial e SHALL ser independentes dos resultados do Simulador (`SimulacaoJogo`).

#### Scenario: Resultado oficial gravado em Jogo
- **WHEN** a aplicação salva o placar oficial de um jogo com ambos os gols informados
- **THEN** `Jogo.GolsMandante` e `Jogo.GolsVisitante` são persistidos no SQLite e ficam disponíveis após o reinício da aplicação

#### Scenario: Resultado oficial removido
- **WHEN** um placar oficial é tornado incompleto
- **THEN** `Jogo.GolsMandante` e `Jogo.GolsVisitante` voltam a nulo e o jogo deixa de contar na classificação oficial

#### Scenario: Independência entre oficial e simulação
- **WHEN** um resultado oficial é gravado em `Jogo`
- **THEN** os registros de `SimulacaoJogo` permanecem inalterados, e vice-versa

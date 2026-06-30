# jogos

## MODIFIED Requirements

### Requirement: Agrupamento por grupo
A listagem SHALL agrupar os jogos pelo grupo do torneio, exibindo os grupos em ordem alfabética (Grupo A a Grupo L). Dentro de cada grupo, os jogos SHALL ser ordenados por data e horário de forma crescente; como os dados oficiais semeados passam a incluir o horário da partida, a ordem entre jogos do mesmo dia SHALL refletir o horário real, com desempate estável e determinístico por `Id` quando dois jogos tiverem exatamente a mesma data e hora. Cada grupo SHALL ser introduzido por um cabeçalho (`JogosGrupoHeader`) com o título do grupo ("Grupo X · Fase de grupos") e a quantidade de jogos.

#### Scenario: Jogos agrupados por grupo
- **WHEN** a listagem é renderizada sem filtro
- **THEN** os jogos aparecem agrupados por grupo, do Grupo A ao Grupo L, cada grupo sob um cabeçalho com o título e a contagem de jogos

#### Scenario: Ordem cronológica dentro do grupo
- **WHEN** existem jogos em datas e horários diferentes dentro de um grupo
- **THEN** eles são exibidos em ordem crescente de data e horário, com desempate determinístico e estável por `Id` quando a data e a hora forem idênticas

### Requirement: Informações de grupo e estádio na partida
Cada partida exibida por `JogoCard` SHALL apresentar as seleções mandante e visitante (nome e bandeira), o **horário** da partida (formato `HH:mm`), o grupo da partida e as informações do estádio (nome do estádio e cidade). O horário SHALL ser exibido junto ao nome do grupo na label da partida (ex.: "16:00 Grupo A").

#### Scenario: Exibição das informações da partida
- **WHEN** um `JogoCard` é renderizado
- **THEN** ele mostra o mandante e o visitante com suas bandeiras, o horário no formato `HH:mm` junto ao grupo, e o estádio com a cidade

#### Scenario: Horário a partir dos dados oficiais
- **WHEN** o horário de uma partida é exibido
- **THEN** ele corresponde ao horário oficial semeado a partir de `/fontes`, sem exibir "00:00" por ausência de horário

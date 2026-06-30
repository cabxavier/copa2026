## Why

A página Grupos já persiste o placar oficial das partidas na entidade `Jogo` (`GolsMandante`/`GolsVisitante`), mas as páginas Jogos e Home (Landing Page) ignoravam esse dado: ambas exibiam sempre status fixo ("AGENDADO"/"PRÓXIMO") e placar vazio "— : —". O resultado registrado pelo usuário ficava invisível fora de Grupos, quebrando a expectativa de que o portal reflita um único estado oficial do torneio.

Esta change é **retroativa**: documenta formalmente, no fluxo OpenSpec, uma funcionalidade que já foi implementada e teve as specs principais sincronizadas.

## What Changes

- A página Jogos (`JogoCard`) passa a exibir o placar oficial persistido na entidade `Jogo`: quando há resultado (ambos os gols preenchidos), mostra status "ENCERRADO" e o placar real (ex.: "2 : 1"); caso contrário, mantém "AGENDADO" e "— : —".
- A Landing Page (`ProximosJogos`) passa a refletir o mesmo placar oficial: "ENCERRADO" + placar quando há resultado; "PRÓXIMO" + "— : —" quando não há.
- `JogoListaDto` e `JogoResumoDto` ganham os campos `GolsMandante`/`GolsVisitante` (nuláveis) e a propriedade `TemResultado`.
- `JogosService` e `LandingPageService` passam a projetar o placar oficial a partir da tabela `Jogo`.
- O Simulador permanece inalterado: continua operando sobre `SimulacaoJogo` (simulações do usuário), sem misturar com o resultado oficial.

## Capabilities

### New Capabilities
<!-- Nenhuma capacidade nova; a funcionalidade estende capacidades existentes. -->

### Modified Capabilities
- `jogos`: o `JogoCard` passa a exibir o resultado oficial persistido (status "ENCERRADO" e placar real) lido exclusivamente da tabela `Jogo`.
- `landing-page`: o componente `ProximosJogos` passa a exibir o resultado oficial persistido (status "ENCERRADO" e placar real) das partidas listadas.

## Impact

- **Código**: `Services/Dtos/JogosDtos.cs`, `Services/Dtos/LandingPageDtos.cs`, `Services/JogosService.cs`, `Services/LandingPageService.cs`, `Components/Pages/Jogos/JogoCard.razor`, `Components/Pages/LandingPage/ProximosJogos.razor`.
- **Specs**: `openspec/specs/jogos/spec.md`, `openspec/specs/landing-page/spec.md` (já sincronizadas).
- **Persistência**: nenhuma alteração de schema — reutiliza `Jogo.GolsMandante`/`Jogo.GolsVisitante` já existentes.
- **Dependências**: nenhuma nova.
- **Compatibilidade**: leitura por carga de página (sem atualização ao vivo entre abas); sem breaking changes.

## Why

O PortalCopa26 ainda não expõe as 48 seleções nem seus elencos: o item "Seleções" do menu está como "em breve". Os dados oficiais de jogadores (`copa2026_selecoes_jogadores.txt`) e técnicos (`copa2026_pais_tecnicos.txt`) já existem em `/fontes`, e a entidade `Jogador` (com a relação `Selecao` 1—N `Jogador`) já está modelada no domínio, mas nunca foi populada nem exibida. Esta change entrega a página Equipes, reaproveitando essa estrutura.

## What Changes

- Nova página **Equipes** (rota `/equipes`, render mode `InteractiveServer`), acessível pelo menu principal (substitui o "Seleções — em breve").
- Listagem das **48 seleções** em cards (bandeira, nome, grupo e código FIFA), com **busca por nome** e **filtro por grupo** (chips "Todos", "Grp A"…"Grp L").
- **Modal de elenco**: ao selecionar uma seleção, abre um modal com bandeira, nome, grupo, técnico e posição no Ranking FIFA, além da tabela do elenco convocado (Nome, Posição, Idade, Gols pela seleção).
- **Seed dos jogadores e técnicos** a partir de `/fontes`, vinculando por código FIFA (`Selecao.Codigo`): ~26 jogadores por seleção e o nome do técnico.
- Novo campo **`Tecnico`** em `Selecao` (e migration correspondente) para persistir o técnico de cada seleção.
- Novo serviço **`SelecaoService`** (camada `Services/`) que retorna DTOs, sem EF Core nos componentes.

## Capabilities

### New Capabilities
- `selecoes`: página Equipes com listagem das 48 seleções, busca, filtro por grupo e modal de elenco (técnico, ranking e jogadores convocados), alimentada por serviço sobre os dados oficiais de `/fontes`.

### Modified Capabilities
<!-- Nenhum requisito de capacidade existente muda. A entidade Jogador e a tabela Jogadores já existiam no domínio; aqui passam a ser populadas e exibidas. -->

## Impact

- **Modelo/Persistência**: `Models/Selecao.cs` ganha `Tecnico` (string?). Nova migration adiciona a coluna `Tecnico` em `Selecoes`. A tabela `Jogadores` já existe (sem mudança de schema). `DbInitializer` passa a semear jogadores e técnicos a partir de `/fontes`.
- **Dados oficiais**: `fontes/copa2026_selecoes_jogadores.txt` e `fontes/copa2026_pais_tecnicos.txt` passam a ser fonte de seed (incorporados ao projeto para leitura no seed).
- **Serviços/DTOs**: novos `Services/ISelecaoService.cs`, `Services/SelecaoService.cs`, `Services/Dtos/SelecoesDtos.cs`; registro em `Program.cs`.
- **UI**: nova `Components/Pages/Selecoes/Equipes.razor` e componentes `CartaoTime`, `FiltroTime`, `ModalTime`, `TabelaElencoTime`; ativação do item "Seleções/Equipes" no menu (`MainLayout.razor`).
- **CSS**: nenhum — as classes do protótipo (`.tgrid`, `.tcard`, `.teamrow`, `.search`, `.chip.orange`, `.modal`, `.pos-tag`) já existem em `wwwroot/css/portal.css`.
- **Fora de escopo**: edição de elencos, estatísticas adicionais e a coluna "Copas" (sem dado na fonte).

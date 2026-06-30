## 1. Modelo e Persistência

- [x] 1.1 Adicionar a propriedade `Tecnico` (string?) em `Models/Selecao.cs`
- [x] 1.2 Refletir `Tecnico` na configuração de `Selecao` (`SelecaoConfiguration`) com tamanho máximo adequado
- [x] 1.3 Criar a migration `AddTecnicoSelecao` (adiciona coluna `Tecnico` em `Selecoes`) e atualizar o snapshot
- [x] 1.4 Incluir `fontes/copa2026_selecoes_jogadores.txt` e `fontes/copa2026_pais_tecnicos.txt` no projeto com cópia para a saída (`CopyToOutputDirectory`) ou como recurso, garantindo leitura por caminho relativo no seed

## 2. Seed de elenco e técnicos

- [x] 2.1 Criar parser dos arquivos de `/fontes`: blocos `# Nome (COD)` + linhas `Nome|Idade|Posicao|Gols` (jogadores) e linhas `Nome (COD)|Técnico` (técnicos)
- [x] 2.2 Mapear a posição textual da fonte para `PosicaoJogador` (Goleiro/Defensor/Meio-campista/Atacante → Goleiro/Defensor/MeioCampo/Atacante)
- [x] 2.3 No `DbInitializer`, popular `Selecao.Tecnico` por código FIFA (`Selecao.Codigo`), com guard de idempotência próprio. O seed de técnicos DEVE executar **fora/antes** do early-return `if (await db.Grupos.AnyAsync()) return;`, com verificação própria (ex.: `if (await db.Selecoes.AnyAsync(s => s.Tecnico == null))`), para popular bancos de desenvolvimento já existentes
- [x] 2.4 No `DbInitializer`, popular `Jogadores` por código FIFA (~26 por seleção), idempotente, sem inventar dados (ParticipacoesCopas permanece 0). Assim como 2.3, o seed de jogadores DEVE executar **fora/antes** do early-return de `Grupos`, com verificação própria (ex.: `if (!await db.Jogadores.AnyAsync())`)
- [x] 2.5 Atualizar o comentário XML-doc do `DbInitializer` para refletir que o seed de elenco/técnicos é lido em runtime dos `.txt` de `/fontes` (e não transcrito em `DadosCopa`); garantir leitura em UTF-8 e caminho relativo válido na saída
- [x] 2.6 Validar a primeira inicialização com banco vazio: 48 seleções com técnico e elenco populados

## 3. Serviço e DTOs

- [x] 3.1 Criar `Services/Dtos/SelecoesDtos.cs` (DTO de card de seleção e DTO de detalhe com técnico, ranking e lista de jogadores)
- [x] 3.2 Criar `Services/ISelecaoService.cs` e `Services/SelecaoService.cs` usando `IDbContextFactory`, retornando DTOs (listagem de seleções + detalhe/elenco por seleção; ranking opcional). A listagem DEVE ser ordenada alfabeticamente com cultura pt-BR (ordenação culture-aware), para tratar nomes acentuados (ex.: "África do Sul", "Áustria", "Argélia") corretamente — não usar ordenação ordinal
- [x] 3.3 Registrar `ISelecaoService` em `Program.cs`

## 4. Página e Componentes

- [x] 4.1 Criar `Components/Pages/Selecoes/Equipes.razor` (rota `/equipes`, `InteractiveServer`): título, subtítulo "48 seleções…", estado de busca/filtro/modal
- [x] 4.2 Criar `Components/Pages/Selecoes/FiltroTime.razor`: campo de busca + chips de grupo ("Todos", "Grp A"…"Grp L"), emitindo eventos de mudança
- [x] 4.3 Criar `Components/Pages/Selecoes/CartaoTime.razor`: card com bandeira (`fl-img`), nome, "Grupo X" e código FIFA; clique abre o modal
- [x] 4.4 Criar `Components/Pages/Selecoes/ModalTime.razor`: cabeçalho (bandeira, nome, "Grupo X · Técnico · Ranking FIFA #N" ou "Sem ranking"), fechar por botão e por clique fora
- [x] 4.5 Criar `Components/Pages/Selecoes/TabelaElencoTime.razor`: tabela com Nome, Posição (etiqueta `pos-GK/DF/MF/FW`), Idade e Gols (`goals`)
- [x] 4.6 Implementar busca (substring, case-insensitive), filtro por grupo e combinação de ambos, com mensagem "nenhuma seleção encontrada"
- [x] 4.7 Ativar o item de menu de seleções em `MainLayout.razor` apontando para `/equipes` e marcar como ativo na rota; remover o estado "em breve". A marcação de "ativo" DEVE derivar do path `equipes` (e não `selecoes`), seguindo o padrão de `OnParametersSet` existente

## 5. Verificação

- [x] 5.1 Compilar a solução (`dotnet build`) sem erros nem avisos
- [x] 5.2 Conferir critérios de aceitação: 48 seleções, ~26 jogadores por seleção, busca, filtro por grupo e modal funcionando, layout semelhante ao protótipo
- [x] 5.3 Validar a change com `openspec validate criar-selecoes-jogadores`

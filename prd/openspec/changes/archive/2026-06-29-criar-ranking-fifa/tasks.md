## 1. Validação dos dados e do seed

- [x] 1.1 Conferir que `Data/Seed/DadosCopa.cs` (`Ranking`) reflete fielmente `fontes/copa2026_ranking_fifa.txt` e mantém a exclusão proposital de Itália (12º) e Dinamarca (20º), sem inventar dados
- [x] 1.2 Confirmar que a entidade `Models/RankingFifa.cs`, a configuração `RankingFifaConfiguration` e o `DbSet<RankingFifa>` atendem à exibição de posição, pontuação, seleção e grupo (via `Selecao.Grupo`) — ajustar somente se necessário

## 2. Serviço e DTO

- [x] 2.1 Criar `Services/Dtos/RankingDtos.cs` com `RankingFifaItemDto(int Posicao, string Selecao, string Codigo, string? BandeiraUrl, decimal Pontuacao, string Grupo)`
- [x] 2.2 Criar `Services/IRankingService.cs` com `Task<IReadOnlyList<RankingFifaItemDto>> ObterRankingAsync()`
- [x] 2.3 Implementar `Services/RankingService.cs` usando `IDbContextFactory`, `AsNoTracking`, `Include(r => r.Selecao).ThenInclude(s => s.Grupo)`, ordenado por `Posicao`, projetando o grupo
- [x] 2.4 Registrar `IRankingService`/`RankingService` na injeção de dependência em `Program.cs`

## 3. Componentes reutilizáveis

- [x] 3.1 Criar `Components/Pages/Ranking/RankingLinha.razor` (posição, bandeira local com fallback `onerror` para PNG ausente, nome, pontuação formatada com 2 casas decimais em pt-BR, grupo; parâmetro de destaque do Top 3)
- [x] 3.2 Criar `Components/Pages/Ranking/RankingTabela.razor` (cabeçalho + lista de `RankingLinha`; estado "nenhuma seleção encontrada")
- [x] 3.3 Criar `Components/Pages/Ranking/RankingTop3.razor` (pódio das posições 1, 2 e 3 com medalhas ouro/prata/bronze, exibindo bandeira, nome, grupo e pontuação)
- [x] 3.4 Criar `Components/Pages/Ranking/RankingPesquisa.razor` (campo de busca; `[Parameter] Termo` + `EventCallback<string> TermoChanged`, filtro em tempo real via `oninput`)

## 4. Página de Ranking

- [x] 4.1 Criar `Components/Pages/Ranking/Ranking.razor` com `@page "/ranking"`, injetar `IRankingService`, carregar os dados e gerenciar o termo de pesquisa
- [x] 4.2 Compor a página com `RankingPesquisa`, `RankingTop3` e `RankingTabela`, aplicando o filtro por nome à lista exibida (mantendo a ordem por posição); exibir o `RankingTop3` apenas quando não houver termo de pesquisa
- [x] 4.3 Reutilizar as classes globais de `wwwroot/css/portal.css` e adicionar uma seção "Página Ranking" no próprio `portal.css` com os estilos do pódio (Top 3) e da tabela (hoje inline em `ranking.html`), mantendo fontes claras/tema escuro e somente Bootstrap 5

## 5. Navegação

- [x] 5.1 Em `Components/Layout/MainLayout.razor`, repontar o link "Ranking" do menu e do rodapé de `/#ranking` para a rota `/ranking`, e adicionar o estado ativo `IsRanking` (espelhando `IsJogos`/`IsGrupos`). `NavMenu.razor` (template default) não é alterado

## 6. Verificação

- [x] 6.1 Compilar a solução (`dotnet build`) e executar a aplicação, validando a rota `/ranking`
- [x] 6.2 Verificar visualmente: ordenação por posição (com salto do 11º para o 13º), pódio do Top 3 (ouro/prata/bronze), colunas posição/seleção (bandeira junto ao nome)/pontuação/grupo, contraste das fontes com o fundo escuro
- [x] 6.3 Testar a pesquisa: filtro por nome, mensagem de "nenhuma seleção encontrada" e retorno à lista completa ao limpar o termo

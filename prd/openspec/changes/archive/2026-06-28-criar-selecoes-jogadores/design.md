## Context

A capacidade Seleções é a última das capacidades principais do CLAUDE.md ainda não entregue. O domínio já tem a entidade `Jogador` (Id, Nome, `PosicaoJogador`, Idade, GolsMarcados, ParticipacoesCopas, SelecaoId) com `JogadorConfiguration` e a relação `Selecao` 1—N `Jogador` (a tabela `Jogadores` já consta no `AppDbContextModelSnapshot`). Faltam: persistir o técnico, popular jogadores/técnicos a partir de `/fontes`, e construir a página/serviço.

As classes CSS do protótipo `equipes.html` (`.tgrid`, `.tcard`, `.teamrow`, `.search`, `.chip.orange`, `.modal`, `.modal-box`, `.modal-head`, `.pos-tag`, `.pos-GK/DF/MF/FW`, `.goals`) já existem em `wwwroot/css/portal.css` — a página apenas reaproveita esse visual.

Fontes de dados:
- `copa2026_selecoes_jogadores.txt`: blocos `# Nome (COD)` seguidos de linhas `Nome|Idade|Posicao|Gols` (~26 por seleção).
- `copa2026_pais_tecnicos.txt`: linhas `Nome (COD)|Técnico`.
Ambos identificam a seleção pelo código FIFA de 3 letras, que corresponde a `Selecao.Codigo`.

## Goals / Non-Goals

**Goals:**
- Página Equipes em `/equipes` com listagem das 48 seleções, busca, filtro por grupo e modal de elenco, fiel ao protótipo.
- Popular elencos e técnicos a partir de `/fontes`, sem invenção e de forma idempotente.
- Seguir o padrão existente: serviço + DTOs, sem EF Core nos componentes; `IDbContextFactory` de vida curta.

**Non-Goals:**
- Edição/CRUD de seleções ou elencos.
- Coluna "Copas"/participações em Copas (a fonte de jogadores não fornece esse dado; `ParticipacoesCopas` permanece 0 e não é exibida).
- Estatísticas agregadas por seleção além do que o modal pede.
- Alterações de CSS (já existente).

## Decisions

- **Vincular por código FIFA, não por nome.** Os arquivos de `/fontes` usam nomes em inglês (ex.: "Algeria"), enquanto `Selecao.Nome` é em português ("Argélia"). A junção usa `Selecao.Codigo` (ex.: "ALG"), que é idêntico nos dois lados. Alternativa descartada: tabela de tradução de nomes — frágil e desnecessária, pois o código já é único e estável.

- **Persistir o técnico como `Selecao.Tecnico` (string?).** Simples, coeso com a seleção e suficiente para o modal. Requer uma migration adicionando a coluna `Tecnico` em `Selecoes`. Alternativa descartada: entidade `Tecnico` separada — excesso de estrutura para um único campo textual (1—1 com seleção), violaria "não criar novas arquiteturas".

- **Seed lendo os arquivos de `/fontes` como recurso do projeto, não transcrição manual.** São ~1.250 linhas de jogadores; transcrevê-las em constantes C# (como `DadosCopa`) seria volumoso e propenso a erros. Os dois `.txt` serão incluídos no projeto (copiados para a saída via `CopyToOutputDirectory`/conteúdo) e parseados no `DbInitializer`, preservando `/fontes` como fonte de verdade. Um parser dedicado (`DadosElenco`/método no seed) interpreta os blocos e linhas. Alternativa descartada: transcrição para `DadosCopa.cs` — mantém o padrão de constantes, mas o custo/risco do volume não compensa; a leitura de arquivo no seed não introduz nova arquitetura.

- **Mapa de posição fonte→enum→CSS.** "Goleiro"→`Goleiro`→`pos-GK`; "Defensor"→`Defensor`→`pos-DF`; "Meio-campista"→`MeioCampo`→`pos-MF`; "Atacante"→`Atacante`→`pos-FW`. O enum é persistido como string (conversão já configurada). A etiqueta exibida usa abreviação coerente com a classe CSS.

- **Bandeiras como imagem real (`fl-img` via `BandeiraUrl`), não emoji.** O protótipo usa emoji em `.tcard .fl`, mas todo o portal já usa imagens locais de bandeira. Manter `BandeiraUrl` é consistente com Jogos/Grupos/Home.

- **Ranking opcional no modal.** O Ranking FIFA tem cobertura parcial (18 de 48 seleções, por fidelidade à fonte). Quando ausente, o modal exibe "Sem ranking" em vez de número inventado.

- **Busca e filtro no cliente, sobre a lista já carregada.** As 48 seleções são poucas; a página carrega todas uma vez e aplica busca/filtro em memória (estado no componente `Equipes`), evitando idas repetidas ao banco. O `SelecaoService` expõe a listagem e o detalhe (elenco) por seleção.

- **Modal em Blazor por estado, sem JSInterop.** A visibilidade do modal é controlada por estado C# (classe `open` condicional) e `@onclick`, coerente com "JSInterop apenas quando necessário".

## Risks / Trade-offs

- [Arquivos de `/fontes` não chegarem à saída de execução do seed] → mitigação: marcar os dois `.txt` com `CopyToOutputDirectory` (ou incorporá-los como recurso) e validar o caminho relativo no `DbInitializer`; testar a primeira inicialização com banco vazio.
- [Código FIFA divergente entre fonte e `DadosCopa`] → mitigação: o seed registra/loga seleções sem correspondência; a expectativa é casar as 48. Critério de aceitação cobre as 48 seleções e ~26 jogadores cada.
- [Banco já existente em ambientes de desenvolvimento não re-seedará (idempotência por "tabela não vazia")] → mitigação: como `Jogadores`/`Tecnico` são novidades, o seed de elenco/técnico deve ter verificação própria (ex.: "se não há jogadores, popular"), independente do guard de `Grupos`, para popular bancos já criados sem recriar o arquivo.
- [Parsing frágil a linhas em branco/acentuação] → mitigação: parser tolerante a linhas vazias e a `#` de cabeçalho; leitura em UTF-8.

## Migration Plan

1. Adicionar `Tecnico` a `Selecao` e criar migration (`AddTecnicoSelecao`) que adiciona a coluna `Tecnico` em `Selecoes` (nullable). A tabela `Jogadores` já existe no schema — sem mudança.
2. Atualizar `DbInitializer` para, após semear seleções, popular técnicos (`copa2026_pais_tecnicos.txt`) e jogadores (`copa2026_selecoes_jogadores.txt`) por código FIFA, com guard próprio de idempotência.
3. Rollback: reverter a migration (remove `Tecnico`) e os arquivos de código; jogadores/técnicos semeados são descartáveis (dados oficiais recarregáveis).

## Open Questions

- Rótulo do item de menu: "Seleções" (domínio/CLAUDE.md) ou "Equipes" (protótipo). Decisão adotada: usar "Seleções" como rótulo apontando para `/equipes`, preservando o vocabulário do projeto; ajustável sem impacto funcional.

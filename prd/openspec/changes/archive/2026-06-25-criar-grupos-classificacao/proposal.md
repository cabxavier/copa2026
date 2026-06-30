## Why

A página **Grupos** existe apenas como protótipo estático (`../prototipo/grupos.html`), com classificações e jogos fixos em `0`. O banco SQLite já possui os 12 grupos, as 48 seleções e os 72 jogos oficiais da fase de grupos, mas não há tela que apresente a classificação real nem permita registrar os resultados oficiais. Esta change transforma o protótipo em funcionalidade real, calculando a classificação dinamicamente a partir dos resultados oficiais e permitindo registrá-los/atualizá-los diretamente na página.

## What Changes

- Nova página interativa `/grupos` (Interactive Server) fiel ao protótipo: cabeçalho da página (título + subtítulo) com botão **Simular** à direita, chips de navegação dos 12 grupos (A–L) e layout em duas colunas (jogos à esquerda + classificação à direita).
- Exibição da classificação do grupo selecionado com as colunas: Posição, Seleção, Jogos (J), Vitórias (V), Empates (E), Derrotas (D), Gols Pró (GP), Gols Contra (GC), Saldo de Gols (SG) e Pontos (PTS) — GP e GC em colunas separadas.
- Classificação calculada **dinamicamente a partir dos resultados oficiais** registrados na tabela `Jogos`, aplicando os critérios oficiais de desempate dentro do grupo (`fontes/copa2026_regras_negocio.txt`): pontos → saldo de gols → gols marcados → confronto direto → (fair play / sorteio como ordem estável determinística).
- Destaque dos 2 primeiros colocados (classificados), do 3º (melhor terceiro) e dos eliminados (4º), com legenda de 3 itens no rodapé do painel de classificação — fidelidade visual ao protótipo.
- Edição dos **resultados oficiais** dos jogos da fase de grupos diretamente na página: ao salvar (ação explícita por jogo, com confirmação visual "✓ salvo"), persiste os gols na tabela `Jogos`, recalcula a classificação e reflete imediatamente na interface.
- Botão **Simular** no cabeçalho da página, integrando a página Grupos à página `/simulador`.
- **Reutilização** do serviço de cálculo `CalculadoraClassificacao` (lógica pura, já alinhada à ordem oficial de desempate) e do componente `ClassificacaoGrupo.razor`, evitando duplicação da lógica de classificação.
- Isolamento explícito: a classificação oficial usa **somente** os placares da tabela `Jogos`; os placares do Simulador (tabela `SimulacaoJogos`) permanecem independentes e não interferem na classificação oficial.

## Capabilities

### New Capabilities
- `grupos`: Página de consulta dos grupos da Copa 2026 — navegação por grupo, classificação oficial calculada dinamicamente, jogos do grupo, registro/atualização de resultados oficiais e integração com o Simulador.

### Modified Capabilities
- `persistencia-dados`: Os campos de placar oficial de `Jogo` (`GolsMandante`/`GolsVisitante`) passam a ser graváveis pela aplicação (registro e atualização de resultados oficiais da fase de grupos), permanecendo independentes dos resultados do Simulador.

## Impact

- **Nova capability/specs**: `grupos`; delta em `persistencia-dados`.
- **UI**: novos componentes em `Components/Pages/Grupos/` e habilitação da navegação para `/grupos` no cabeçalho real (`MainLayout`) e nos botões "Ver Grupos" (página Jogos e painel da Landing Page), antes desabilitados.
- **Serviços**: novo `IGruposService`/`GruposService`. Reúso de `CalculadoraClassificacao`. O componente `ClassificacaoGrupo.razor` é **promovido a compartilhado** (`Components/Pages/Simulador/` → `Components/Shared/`) e estendido com parâmetros opcionais (colunas GP/GC separadas, marcação de eliminados, legenda configurável, slot de ações e slot de rodapé), preservando o uso atual do Simulador.
- **DTOs**: novos DTOs em `Services/Dtos` para jogos editáveis do grupo; reúso de `ClassificacaoLinhaDto`.
- **CSS**: novo bloco de estilos da página Grupos em `wwwroot/css/portal.css`, escopado em `.grupos` onde necessário para não afetar Simulador/Jogos (cabeçalho da página, botão Simular, caixa de placar e inputs, botão Salvar, tag "✓ salvo", colunas V/D coloridas e marcação de eliminado).
- **Dados**: nenhuma migração de esquema — `Jogo.GolsMandante`/`GolsVisitante` já existem e são anuláveis.
- **Sem mudança** no Simulador, que continua usando `SimulacaoJogos`.

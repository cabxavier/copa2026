> Change retroativa: a implementação já estava concluída e compilando no momento da criação destes artefatos. As tarefas refletem o trabalho realizado.

## 1. DTOs

- [x] 1.1 Adicionar `GolsMandante`/`GolsVisitante` (nuláveis) e a propriedade `TemResultado` a `JogoListaDto` em `Services/Dtos/JogosDtos.cs`
- [x] 1.2 Adicionar `GolsMandante`/`GolsVisitante` (nuláveis) e a propriedade `TemResultado` a `JogoResumoDto` em `Services/Dtos/LandingPageDtos.cs`

## 2. Serviços

- [x] 2.1 Projetar `j.GolsMandante`/`j.GolsVisitante` em `JogosService.ObterJogosAsync`
- [x] 2.2 Projetar `j.GolsMandante`/`j.GolsVisitante` em `LandingPageService.ProjetarAsync` (próximos jogos)

## 3. Componentes

- [x] 3.1 Em `Components/Pages/Jogos/JogoCard.razor`, exibir "ENCERRADO" + placar quando `TemResultado`; senão "AGENDADO" + "— : —"
- [x] 3.2 Em `Components/Pages/LandingPage/ProximosJogos.razor`, exibir "ENCERRADO" + placar quando `TemResultado`; senão "PRÓXIMO" + "— : —"

## 4. Specs

- [x] 4.1 Adicionar requisito "Exibição do resultado oficial persistido" em `openspec/specs/jogos/spec.md`
- [x] 4.2 Atualizar requisito "Próximos jogos" em `openspec/specs/landing-page/spec.md`

## 5. Verificação

- [x] 5.1 Compilar a solução (`dotnet build`) sem erros nem avisos
- [x] 5.2 Confirmar que o Simulador (`SimulacaoJogo`) permaneceu inalterado

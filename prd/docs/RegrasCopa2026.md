# Regras Copa 2026

## Objetivo

Este documento define as regras de negócio da Copa do Mundo FIFA 2026 utilizadas pelo PortalCopa26.

---

## Estrutura da Competição

* 48 seleções participantes
* 12 grupos (A até L)
* 4 seleções por grupo
* 72 jogos na fase de grupos
* 104 jogos no total

---

## Fase de Grupos

Cada grupo possui 4 seleções.

Cada seleção realiza 3 partidas.

---

## Classificação dos Grupos

A classificação deve ser calculada a partir dos resultados dos jogos.

Pontuação:

* Vitória = 3 pontos
* Empate = 1 ponto
* Derrota = 0 ponto

Exibir:

* Jogos
* Vitórias
* Empates
* Derrotas
* Gols Pró
* Gols Contra
* Saldo de Gols
* Pontos

---

## Critérios de Desempate

Ordem de aplicação (conforme `./fontes/copa2026_regras_negocio.txt`):

1. Pontos
2. Saldo de gols
3. Gols marcados
4. Confronto direto entre as seleções empatadas
5. Fair Play (menos cartões)
6. Sorteio pela FIFA

Observação:

A primeira versão da aplicação implementa os quatro primeiros critérios. Fair Play (cartões) e sorteio não são modelados — não há dados de cartões — e recaem em uma ordenação estável e determinística (por `SelecaoId`).

---

## Classificação para o Mata-Mata

Classificam-se:

* Os 2 primeiros colocados de cada grupo
* Os 8 melhores terceiros colocados

Total:

* 32 seleções classificadas

---

## Resultados Oficiais

Os resultados oficiais são a única fonte de verdade para:

- Classificação dos grupos
- Estatísticas oficiais
- Exibição dos resultados na página Jogos
- Definição dos classificados para as fases eliminatórias

---

## Simulações

Os resultados simulados nunca devem alterar:

- Jogos oficiais
- Classificação oficial
- Estatísticas oficiais

As classificações geradas pelo simulador devem utilizar apenas os resultados simulados.

---

## Mata-Mata

Em caso de empate:

1. Prorrogação
2. Pênaltis

Não existe gol de ouro ou gol de prata.

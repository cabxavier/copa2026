using PortalCopa26.Services.Dtos;

namespace PortalCopa26.Services;

/// <summary>
/// Lógica pura (sem acesso a dados) de classificação da fase de grupos.
/// Recebe os jogos do grupo com placares simulados e devolve as linhas
/// ordenadas pelos critérios oficiais de desempate da FIFA dentro do grupo:
/// pontos → saldo de gols → gols marcados → confronto direto.
/// Isolada para ser testável sem banco.
/// </summary>
public static class CalculadoraClassificacao
{
    private sealed class Stats
    {
        public int SelecaoId;
        public string Nome = string.Empty;
        public string? Bandeira;
        public int J, V, E, D, GP, GC;
        public int Pontos => V * 3 + E;
        public int Saldo => GP - GC;
    }

    /// <summary>
    /// Calcula a classificação do grupo. Jogos sem ambos os placares preenchidos
    /// são ignorados na contagem. Vitória = 3, empate = 1, derrota = 0.
    /// </summary>
    public static IReadOnlyList<ClassificacaoLinhaDto> Calcular(IEnumerable<SimuladorJogoDto> jogos)
    {
        var lista = jogos.ToList();
        var tab = new Dictionary<int, Stats>();

        Stats Obter(int id, string nome, string? bandeira)
        {
            if (!tab.TryGetValue(id, out var s))
            {
                s = new Stats { SelecaoId = id, Nome = nome, Bandeira = bandeira };
                tab[id] = s;
            }
            return s;
        }

        foreach (var j in lista)
        {
            // Garante que as 4 seleções apareçam mesmo sem jogos computados.
            var h = Obter(j.MandanteId, j.MandanteNome, j.MandanteBandeira);
            var a = Obter(j.VisitanteId, j.VisitanteNome, j.VisitanteBandeira);

            if (j.GolsMandante is not int gm || j.GolsVisitante is not int gv)
                continue;

            h.J++; a.J++;
            h.GP += gm; h.GC += gv;
            a.GP += gv; a.GC += gm;
            if (gm > gv) { h.V++; a.D++; }
            else if (gm < gv) { a.V++; h.D++; }
            else { h.E++; a.E++; }
        }

        // Critérios 1-3: pontos, saldo, gols marcados.
        var ordenado = tab.Values
            .OrderByDescending(s => s.Pontos)
            .ThenByDescending(s => s.Saldo)
            .ThenByDescending(s => s.GP)
            .ToList();

        // Critério 4 (confronto direto) aplicado por cluster ainda empatado.
        var resultado = new List<Stats>(ordenado.Count);
        var i = 0;
        while (i < ordenado.Count)
        {
            var k = i + 1;
            while (k < ordenado.Count && Empatados(ordenado[i], ordenado[k]))
                k++;

            if (k - i == 1)
                resultado.Add(ordenado[i]);
            else
                resultado.AddRange(DesempatarPorConfrontoDireto(ordenado.GetRange(i, k - i), lista));

            i = k;
        }

        return resultado
            .Select((s, idx) => new ClassificacaoLinhaDto(
                idx + 1, s.SelecaoId, s.Nome, s.Bandeira,
                s.J, s.V, s.E, s.D, s.GP, s.GC, s.Saldo, s.Pontos))
            .ToList();
    }

    private static bool Empatados(Stats a, Stats b) =>
        a.Pontos == b.Pontos && a.Saldo == b.Saldo && a.GP == b.GP;

    /// <summary>
    /// Resolve um grupo de seleções empatadas pelos confrontos entre elas
    /// (mini-tabela): pontos → saldo → gols marcados considerando apenas os
    /// jogos disputados entre as empatadas. Empate residual recai em ordem
    /// estável por <c>SelecaoId</c>, garantindo determinismo.
    /// </summary>
    private static IEnumerable<Stats> DesempatarPorConfrontoDireto(
        List<Stats> empatadas, List<SimuladorJogoDto> jogos)
    {
        var ids = empatadas.Select(s => s.SelecaoId).ToHashSet();
        var pts = empatadas.ToDictionary(s => s.SelecaoId, _ => 0);
        var saldo = empatadas.ToDictionary(s => s.SelecaoId, _ => 0);
        var gp = empatadas.ToDictionary(s => s.SelecaoId, _ => 0);

        foreach (var j in jogos)
        {
            if (j.GolsMandante is not int gm || j.GolsVisitante is not int gv)
                continue;
            if (!ids.Contains(j.MandanteId) || !ids.Contains(j.VisitanteId))
                continue;

            gp[j.MandanteId] += gm; gp[j.VisitanteId] += gv;
            saldo[j.MandanteId] += gm - gv; saldo[j.VisitanteId] += gv - gm;
            if (gm > gv) pts[j.MandanteId] += 3;
            else if (gm < gv) pts[j.VisitanteId] += 3;
            else { pts[j.MandanteId] += 1; pts[j.VisitanteId] += 1; }
        }

        return empatadas
            .OrderByDescending(s => pts[s.SelecaoId])
            .ThenByDescending(s => saldo[s.SelecaoId])
            .ThenByDescending(s => gp[s.SelecaoId])
            .ThenBy(s => s.SelecaoId);
    }
}

using PortalCopa26.Services;
using PortalCopa26.Services.Dtos;
using Xunit;

namespace PortalCopa26.Tests;

/// <summary>
/// Testes da lógica pura de classificação e desempate da fase de grupos.
/// Grupo de 4 seleções (ids 1..4) com os 6 jogos do round-robin:
/// 1:(1×2) 2:(1×3) 3:(1×4) 4:(2×3) 5:(2×4) 6:(3×4).
/// </summary>
public class CalculadoraClassificacaoTests
{
    private static SimuladorJogoDto J(int id, int mandante, int visitante, int? gm, int? gv)
        => new(id, mandante, $"T{mandante}", null, visitante, $"T{visitante}", null, gm, gv);

    // ---- Pontuação e contagem ----

    [Fact]
    public void Vitoria_Empate_Derrota_pontuam_3_1_0()
    {
        // T1 vence T2 (2x0); T3 e T4 empatam (1x1); demais sem placar.
        var jogos = new List<SimuladorJogoDto>
        {
            J(1, 1, 2, 2, 0),
            J(6, 3, 4, 1, 1),
            J(2, 1, 3, null, null),
            J(3, 1, 4, null, null),
            J(4, 2, 3, null, null),
            J(5, 2, 4, null, null),
        };

        var r = CalculadoraClassificacao.Calcular(jogos);
        var t1 = r.Single(x => x.SelecaoId == 1);
        var t2 = r.Single(x => x.SelecaoId == 2);
        var t3 = r.Single(x => x.SelecaoId == 3);

        Assert.Equal((1, 1, 0, 0, 2, 0, 2, 3), (t1.J, t1.V, t1.E, t1.D, t1.GolsPro, t1.GolsContra, t1.Saldo, t1.Pontos));
        Assert.Equal((1, 0, 0, 1, 0), (t2.J, t2.V, t2.E, t2.D, t2.Pontos));
        Assert.Equal((1, 0, 1, 0, 1), (t3.J, t3.V, t3.E, t3.D, t3.Pontos));
    }

    [Fact]
    public void Todas_as_quatro_selecoes_aparecem_mesmo_sem_jogos_computados()
    {
        var jogos = TodosNulos();

        var r = CalculadoraClassificacao.Calcular(jogos);

        Assert.Equal(4, r.Count);
        Assert.All(r, linha => Assert.Equal(0, linha.Pontos));
        // Sem nada para desempatar, ordem estável por SelecaoId.
        Assert.Equal(new[] { 1, 2, 3, 4 }, r.Select(x => x.SelecaoId).ToArray());
        Assert.Equal(new[] { 1, 2, 3, 4 }, r.Select(x => x.Posicao).ToArray());
    }

    [Fact]
    public void Jogo_com_placar_parcial_nao_computa()
    {
        var jogos = TodosNulos();
        jogos[0] = J(1, 1, 2, 3, null); // só o mandante preenchido

        var r = CalculadoraClassificacao.Calcular(jogos);

        Assert.All(r, linha => Assert.Equal(0, linha.J));
        Assert.All(r, linha => Assert.Equal(0, linha.Pontos));
    }

    // ---- Critérios de desempate ----

    [Fact]
    public void Desempate_por_saldo_de_gols()
    {
        // T1 e T2 com 3 pts; T1 venceu por 3x0, T2 por 1x0.
        var jogos = TodosNulos();
        jogos[2] = J(3, 1, 4, 3, 0); // T1 3x0 T4
        jogos[4] = J(5, 2, 4, 1, 0); // T2 1x0 T4

        var r = CalculadoraClassificacao.Calcular(jogos);

        var t1 = r.Single(x => x.SelecaoId == 1);
        var t2 = r.Single(x => x.SelecaoId == 2);
        Assert.Equal(3, t1.Pontos);
        Assert.Equal(3, t2.Pontos);
        Assert.True(t1.Posicao < t2.Posicao, "Maior saldo (T1) deve ficar à frente");
    }

    [Fact]
    public void Desempate_por_gols_marcados_quando_saldo_igual()
    {
        // T1 e T2: 3 pts e saldo +2; T2 marcou mais (3) que T1 (2).
        var jogos = TodosNulos();
        jogos[2] = J(3, 1, 4, 2, 0); // T1 2x0 T4  (sg+2, gp2)
        jogos[4] = J(5, 2, 4, 3, 1); // T2 3x1 T4  (sg+2, gp3)

        var r = CalculadoraClassificacao.Calcular(jogos);

        var t1 = r.Single(x => x.SelecaoId == 1);
        var t2 = r.Single(x => x.SelecaoId == 2);
        Assert.Equal(t1.Saldo, t2.Saldo);
        Assert.True(t2.Posicao < t1.Posicao, "Mais gols marcados (T2) deve ficar à frente");
    }

    [Fact]
    public void Desempate_por_confronto_direto_entre_duas_selecoes()
    {
        // Construção em que T1 e T2 terminam empatados em pts(3), saldo(-1) e gols(1),
        // e apenas o confronto direto (T1 venceu T2) os separa. T3 e T4 ficam com 6 pts.
        var jogos = new List<SimuladorJogoDto>
        {
            J(1, 1, 2, 1, 0), // T1 1x0 T2  (confronto direto: T1 vence)
            J(2, 1, 3, 0, 1), // T3 vence T1
            J(3, 1, 4, 0, 1), // T4 vence T1
            J(4, 2, 3, 1, 0), // T2 vence T3
            J(5, 2, 4, 0, 1), // T4 vence T2
            J(6, 3, 4, 2, 0), // T3 vence T4
        };

        var r = CalculadoraClassificacao.Calcular(jogos);
        var t1 = r.Single(x => x.SelecaoId == 1);
        var t2 = r.Single(x => x.SelecaoId == 2);

        // Mesma pontuação, saldo e gols marcados.
        Assert.Equal((t2.Pontos, t2.Saldo, t2.GolsPro), (t1.Pontos, t1.Saldo, t1.GolsPro));
        Assert.Equal(3, t1.Pontos);
        Assert.True(t1.Posicao < t2.Posicao, "Vencedor do confronto direto (T1) deve ficar à frente");
        // T3 e T4 (6 pts) ficam no topo.
        Assert.Equal(new[] { 3, 4, 1, 2 }, r.Select(x => x.SelecaoId).ToArray());
    }

    [Fact]
    public void Empate_triplo_em_ciclo_cai_em_ordem_estavel_por_SelecaoId()
    {
        // T1 vence as 3; T2, T3, T4 formam ciclo (T2>T3, T3>T4, T4>T2), todos 3 pts/sg-1.
        var jogos = new List<SimuladorJogoDto>
        {
            J(1, 1, 2, 1, 0), // T1 vence T2
            J(2, 1, 3, 1, 0), // T1 vence T3
            J(3, 1, 4, 1, 0), // T1 vence T4
            J(4, 2, 3, 1, 0), // T2 vence T3
            J(6, 3, 4, 1, 0), // T3 vence T4
            J(5, 2, 4, 0, 1), // T4 vence T2
        };

        var r = CalculadoraClassificacao.Calcular(jogos);

        Assert.Equal(9, r[0].Pontos);
        Assert.Equal(1, r[0].SelecaoId);
        // Empate triplo indecidível (ciclo): mini-tabela não resolve -> ordem por SelecaoId.
        Assert.Equal(new[] { 1, 2, 3, 4 }, r.Select(x => x.SelecaoId).ToArray());
        Assert.All(r.Skip(1), linha =>
        {
            Assert.Equal(3, linha.Pontos);
            Assert.Equal(-1, linha.Saldo);
        });
    }

    [Fact]
    public void Resultado_e_deterministico_entre_execucoes()
    {
        var jogos = new List<SimuladorJogoDto>
        {
            J(1, 1, 2, 1, 0),
            J(2, 1, 3, 1, 0),
            J(3, 1, 4, 1, 0),
            J(4, 2, 3, 1, 0),
            J(6, 3, 4, 1, 0),
            J(5, 2, 4, 0, 1),
        };

        var a = CalculadoraClassificacao.Calcular(jogos).Select(x => x.SelecaoId).ToArray();
        var b = CalculadoraClassificacao.Calcular(jogos).Select(x => x.SelecaoId).ToArray();
        Assert.Equal(a, b);
    }

    private static List<SimuladorJogoDto> TodosNulos() => new()
    {
        J(1, 1, 2, null, null),
        J(2, 1, 3, null, null),
        J(3, 1, 4, null, null),
        J(4, 2, 3, null, null),
        J(5, 2, 4, null, null),
        J(6, 3, 4, null, null),
    };
}

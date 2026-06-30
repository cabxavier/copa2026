using System.Globalization;
using PortalCopa26.Models;

namespace PortalCopa26.Data.Seed;

/// <summary>Jogador transcrito de copa2026_selecoes_jogadores.txt.</summary>
public record JogadorSeed(string Nome, int Idade, PosicaoJogador Posicao, int Gols);

/// <summary>
/// Parser dos arquivos oficiais de elenco e técnicos de /fontes. Lê em UTF-8 e
/// vincula cada registro à seleção pelo código FIFA de 3 letras (ex.: "ALG"),
/// extraído do trecho "Nome (COD)" presente em ambos os arquivos. Não inventa
/// dados: apenas transcreve o conteúdo das fontes oficiais.
/// </summary>
public static class ElencoParser
{
    /// <summary>
    /// Lê os técnicos de <c>copa2026_pais_tecnicos.txt</c> (linhas
    /// <c>Nome (COD)|Técnico</c>), retornando um mapa código FIFA → técnico.
    /// </summary>
    public static IReadOnlyDictionary<string, string> ParseTecnicos(string caminho)
    {
        var tecnicos = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var linha in LerLinhas(caminho))
        {
            var partes = linha.Split('|', 2);
            if (partes.Length < 2) continue;

            var codigo = ExtrairCodigo(partes[0]);
            var tecnico = partes[1].Trim();
            if (codigo is null || tecnico.Length == 0) continue;

            tecnicos[codigo] = tecnico;
        }

        return tecnicos;
    }

    /// <summary>
    /// Lê os elencos de <c>copa2026_selecoes_jogadores.txt</c> (blocos
    /// <c># Nome (COD)</c> seguidos de linhas <c>Nome|Idade|Posicao|Gols</c>),
    /// retornando um mapa código FIFA → lista de jogadores.
    /// </summary>
    public static IReadOnlyDictionary<string, List<JogadorSeed>> ParseJogadores(string caminho)
    {
        var elencos = new Dictionary<string, List<JogadorSeed>>(StringComparer.OrdinalIgnoreCase);
        List<JogadorSeed>? atual = null;

        foreach (var linha in LerLinhas(caminho))
        {
            if (linha.StartsWith('#'))
            {
                // Cabeçalho de bloco: "# Nome (COD)".
                var codigo = ExtrairCodigo(linha);
                if (codigo is null) { atual = null; continue; }

                atual = new List<JogadorSeed>();
                elencos[codigo] = atual;
                continue;
            }

            if (atual is null) continue;

            var partes = linha.Split('|');
            if (partes.Length < 4) continue;

            var nome = partes[0].Trim();
            if (!int.TryParse(partes[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var idade)) continue;
            var posicao = MapearPosicao(partes[2].Trim());
            int.TryParse(partes[3].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var gols);

            atual.Add(new JogadorSeed(nome, idade, posicao, gols));
        }

        return elencos;
    }

    /// <summary>
    /// Converte a posição textual da fonte ("Goleiro", "Defensor",
    /// "Meio-campista", "Atacante") no enum <see cref="PosicaoJogador"/>.
    /// </summary>
    public static PosicaoJogador MapearPosicao(string texto) => texto.ToLowerInvariant() switch
    {
        "goleiro" => PosicaoJogador.Goleiro,
        "defensor" => PosicaoJogador.Defensor,
        "meio-campista" => PosicaoJogador.MeioCampo,
        "atacante" => PosicaoJogador.Atacante,
        _ => PosicaoJogador.MeioCampo,
    };

    /// <summary>Extrai o código FIFA do trecho entre parênteses (ex.: "ALG").</summary>
    private static string? ExtrairCodigo(string trecho)
    {
        var abre = trecho.LastIndexOf('(');
        var fecha = trecho.LastIndexOf(')');
        if (abre < 0 || fecha <= abre + 1) return null;

        return trecho.Substring(abre + 1, fecha - abre - 1).Trim();
    }

    /// <summary>Linhas não vazias do arquivo, lidas em UTF-8.</summary>
    private static IEnumerable<string> LerLinhas(string caminho) =>
        File.ReadLines(caminho, System.Text.Encoding.UTF8)
            .Select(l => l.Trim())
            .Where(l => l.Length > 0);
}

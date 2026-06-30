namespace PortalCopa26.Data.Seed;

/// <summary>
/// Dados oficiais da Copa do Mundo 2026 transcritos fielmente da pasta /fontes.
/// Não contém dados fictícios: grupos, seleções e jogos vêm dos arquivos oficiais.
/// Os códigos de país (ISO/FIFA) são identificadores padronizados usados apenas
/// para compor a URL da bandeira.
/// </summary>
public static class DadosCopa
{
    /// <summary>
    /// Bandeira da seleção, servida localmente de wwwroot (baixada da API pública
    /// da FIFA — padrão flags-sq-4). Local evita dependência de rede em runtime.
    /// </summary>
    public static string BandeiraUrl(string codigo) =>
        $"img/flags/{codigo}.png";

    /// <summary>Logo do torneio (FIFA Copa do Mundo 2026), servido de wwwroot.</summary>
    public const string LogoFifaUrl = "img/fifa-logo.png";

    // ----- Constantes oficiais do torneio (copa2026_fases.txt / cidades-sede) -----
    public const int TotalSelecoes = 48;
    public const int TotalGrupos = 12;
    public const int TotalEstadios = 16;
    public const int TotalJogos = 104; // torneio completo (72 fase de grupos + mata-mata)

    /// <summary>Países-sede e número de estádios (copa2026_cidades_sede_estadios.txt).</summary>
    public static readonly IReadOnlyList<PaisSedeSeed> PaisesSede = new[]
    {
        new PaisSedeSeed("Estados Unidos", "USA", 11),
        new PaisSedeSeed("Canadá", "CAN", 2),
        new PaisSedeSeed("México", "MEX", 3),
    };

    public static readonly string[] Grupos =
        { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" };

    /// <summary>48 seleções por grupo (copa2026_grupos.txt) + código de país (ISO/FIFA).</summary>
    public static readonly IReadOnlyList<SelecaoSeed> Selecoes = new[]
    {
        // Grupo A
        new SelecaoSeed("México", "MEX", "A"),
        new SelecaoSeed("África do Sul", "RSA", "A"),
        new SelecaoSeed("Coreia do Sul", "KOR", "A"),
        new SelecaoSeed("República Tcheca", "CZE", "A"),
        // Grupo B
        new SelecaoSeed("Canadá", "CAN", "B"),
        new SelecaoSeed("Bósnia e Herzegovina", "BIH", "B"),
        new SelecaoSeed("Catar", "QAT", "B"),
        new SelecaoSeed("Suíça", "SUI", "B"),
        // Grupo C
        new SelecaoSeed("Brasil", "BRA", "C"),
        new SelecaoSeed("Marrocos", "MAR", "C"),
        new SelecaoSeed("Escócia", "SCO", "C"),
        new SelecaoSeed("Haiti", "HAI", "C"),
        // Grupo D
        new SelecaoSeed("Estados Unidos", "USA", "D"),
        new SelecaoSeed("Paraguai", "PAR", "D"),
        new SelecaoSeed("Austrália", "AUS", "D"),
        new SelecaoSeed("Turquia", "TUR", "D"),
        // Grupo E
        new SelecaoSeed("Alemanha", "GER", "E"),
        new SelecaoSeed("Curaçao", "CUW", "E"),
        new SelecaoSeed("Costa do Marfim", "CIV", "E"),
        new SelecaoSeed("Equador", "ECU", "E"),
        // Grupo F
        new SelecaoSeed("Holanda", "NED", "F"),
        new SelecaoSeed("Japão", "JPN", "F"),
        new SelecaoSeed("Suécia", "SWE", "F"),
        new SelecaoSeed("Tunísia", "TUN", "F"),
        // Grupo G
        new SelecaoSeed("Bélgica", "BEL", "G"),
        new SelecaoSeed("Egito", "EGY", "G"),
        new SelecaoSeed("Irã", "IRN", "G"),
        new SelecaoSeed("Nova Zelândia", "NZL", "G"),
        // Grupo H
        new SelecaoSeed("Espanha", "ESP", "H"),
        new SelecaoSeed("Uruguai", "URU", "H"),
        new SelecaoSeed("Arábia Saudita", "KSA", "H"),
        new SelecaoSeed("Cabo Verde", "CPV", "H"),
        // Grupo I
        new SelecaoSeed("França", "FRA", "I"),
        new SelecaoSeed("Senegal", "SEN", "I"),
        new SelecaoSeed("Noruega", "NOR", "I"),
        new SelecaoSeed("Iraque", "IRQ", "I"),
        // Grupo J
        new SelecaoSeed("Argentina", "ARG", "J"),
        new SelecaoSeed("Argélia", "ALG", "J"),
        new SelecaoSeed("Áustria", "AUT", "J"),
        new SelecaoSeed("Jordânia", "JOR", "J"),
        // Grupo K
        new SelecaoSeed("Portugal", "POR", "K"),
        new SelecaoSeed("Colômbia", "COL", "K"),
        new SelecaoSeed("Uzbequistão", "UZB", "K"),
        new SelecaoSeed("RD Congo", "COD", "K"),
        // Grupo L
        new SelecaoSeed("Inglaterra", "ENG", "L"),
        new SelecaoSeed("Croácia", "CRO", "L"),
        new SelecaoSeed("Gana", "GHA", "L"),
        new SelecaoSeed("Panamá", "PAN", "L"),
    };

    /// <summary>
    /// Ranking FIFA (copa2026_ranking_fifa.txt) — apenas as seleções presentes no
    /// arquivo que também disputam a Copa. Itália (12º) e Dinamarca (20º) não estão
    /// na Copa e por isso não constam aqui. Cobertura parcial proposital (sem invenção).
    /// </summary>
    public static readonly IReadOnlyList<RankingSeed> Ranking = new[]
    {
        new RankingSeed("Argentina", 1, 1877.72m),
        new RankingSeed("Espanha", 2, 1874.71m),
        new RankingSeed("França", 3, 1870.70m),
        new RankingSeed("Inglaterra", 4, 1828.02m),
        new RankingSeed("Portugal", 5, 1767.85m),
        new RankingSeed("Brasil", 6, 1765.86m),
        new RankingSeed("Marrocos", 7, 1755.10m),
        new RankingSeed("Holanda", 8, 1753.57m),
        new RankingSeed("Bélgica", 9, 1742.24m),
        new RankingSeed("Alemanha", 10, 1735.77m),
        new RankingSeed("Croácia", 11, 1720.07m),
        new RankingSeed("Uruguai", 13, 1702.35m),
        new RankingSeed("Colômbia", 14, 1697.92m),
        new RankingSeed("México", 15, 1685.68m),
        new RankingSeed("Senegal", 16, 1682.54m),
        new RankingSeed("Estados Unidos", 17, 1678.82m),
        new RankingSeed("Japão", 18, 1661.38m),
        new RankingSeed("Suíça", 19, 1652.43m),
    };

    /// <summary>
    /// 72 jogos da fase de grupos (copa2026_jogos_primeira_fase.txt), todos com
    /// data, horário, estádio e cidade conforme a fonte (horários no fuso de Brasília).
    /// </summary>
    public static readonly IReadOnlyList<JogoSeed> Jogos = new[]
    {
        // Grupo A
        new JogoSeed("2026-06-11", "16h00", "México", "África do Sul", "A", "Estádio Azteca", "Cidade do México"),
        new JogoSeed("2026-06-11", "23h00", "Coreia do Sul", "República Tcheca", "A", "Estadio Akron", "Guadalajara"),
        new JogoSeed("2026-06-18", "13h00", "República Tcheca", "África do Sul", "A", "Mercedes-Benz Stadium", "Atlanta"),
        new JogoSeed("2026-06-19", "00h00", "México", "Coreia do Sul", "A", "Estadio Akron", "Guadalajara"),
        new JogoSeed("2026-06-24", "22h00", "República Tcheca", "México", "A", "Estádio Azteca", "Cidade do México"),
        new JogoSeed("2026-06-24", "22h00", "África do Sul", "Coreia do Sul", "A", "Estadio BBVA", "Monterrey"),
        // Grupo B
        new JogoSeed("2026-06-12", "16h00", "Canadá", "Bósnia e Herzegovina", "B", "BMO Field", "Toronto"),
        new JogoSeed("2026-06-13", "16h00", "Catar", "Suíça", "B", "Levi's Stadium", "São Francisco"),
        new JogoSeed("2026-06-18", "16h00", "Suíça", "Bósnia e Herzegovina", "B", "SoFi Stadium", "Los Angeles"),
        new JogoSeed("2026-06-18", "19h00", "Canadá", "Catar", "B", "BC Place", "Vancouver"),
        new JogoSeed("2026-06-24", "16h00", "Suíça", "Canadá", "B", "BC Place", "Vancouver"),
        new JogoSeed("2026-06-24", "16h00", "Bósnia e Herzegovina", "Catar", "B", "Lumen Field", "Seattle"),
        // Grupo C
        new JogoSeed("2026-06-13", "19h00", "Brasil", "Marrocos", "C", "MetLife Stadium", "Nova Jersey"),
        new JogoSeed("2026-06-13", "22h00", "Haiti", "Escócia", "C", "Gillette Stadium", "Boston"),
        new JogoSeed("2026-06-19", "19h00", "Escócia", "Marrocos", "C", "Gillette Stadium", "Boston"),
        new JogoSeed("2026-06-19", "22h00", "Brasil", "Haiti", "C", "Lincoln Financial Field", "Filadélfia"),
        new JogoSeed("2026-06-24", "19h00", "Escócia", "Brasil", "C", "Hard Rock Stadium", "Miami"),
        new JogoSeed("2026-06-24", "19h00", "Marrocos", "Haiti", "C", "Mercedes-Benz Stadium", "Atlanta"),
        // Grupo D
        new JogoSeed("2026-06-12", "22h00", "Estados Unidos", "Paraguai", "D", "SoFi Stadium", "Los Angeles"),
        new JogoSeed("2026-06-14", "01h00", "Austrália", "Turquia", "D", "BC Place", "Vancouver"),
        new JogoSeed("2026-06-19", "16h00", "Estados Unidos", "Austrália", "D", "Lumen Field", "Seattle"),
        new JogoSeed("2026-06-20", "01h00", "Turquia", "Paraguai", "D", "Levi's Stadium", "São Francisco"),
        new JogoSeed("2026-06-25", "23h00", "Turquia", "Estados Unidos", "D", "SoFi Stadium", "Los Angeles"),
        new JogoSeed("2026-06-25", "23h00", "Paraguai", "Austrália", "D", "Levi's Stadium", "São Francisco"),
        // Grupo E
        new JogoSeed("2026-06-14", "14h00", "Alemanha", "Curaçao", "E", "NRG Stadium", "Houston"),
        new JogoSeed("2026-06-14", "20h00", "Costa do Marfim", "Equador", "E", "Lincoln Financial Field", "Filadélfia"),
        new JogoSeed("2026-06-20", "17h00", "Alemanha", "Costa do Marfim", "E", "BMO Field", "Toronto"),
        new JogoSeed("2026-06-20", "21h00", "Equador", "Curaçao", "E", "GEHA Field at Arrowhead", "Kansas City"),
        new JogoSeed("2026-06-25", "17h00", "Equador", "Alemanha", "E", "MetLife Stadium", "Nova Jersey"),
        new JogoSeed("2026-06-25", "17h00", "Curaçao", "Costa do Marfim", "E", "Lincoln Financial Field", "Filadélfia"),
        // Grupo F
        new JogoSeed("2026-06-14", "17h00", "Holanda", "Japão", "F", "AT&T Stadium", "Dallas"),
        new JogoSeed("2026-06-14", "23h00", "Suécia", "Tunísia", "F", "Estadio BBVA", "Monterrey"),
        new JogoSeed("2026-06-20", "14h00", "Holanda", "Suécia", "F", "NRG Stadium", "Houston"),
        new JogoSeed("2026-06-21", "01h00", "Tunísia", "Japão", "F", "Estadio BBVA", "Monterrey"),
        new JogoSeed("2026-06-25", "20h00", "Japão", "Suécia", "F", "AT&T Stadium", "Dallas"),
        new JogoSeed("2026-06-25", "20h00", "Tunísia", "Holanda", "F", "GEHA Field at Arrowhead", "Kansas City"),
        // Grupo G
        new JogoSeed("2026-06-15", "19h00", "Bélgica", "Egito", "G", "Lumen Field", "Seattle"),
        new JogoSeed("2026-06-16", "01h00", "Irã", "Nova Zelândia", "G", "SoFi Stadium", "Los Angeles"),
        new JogoSeed("2026-06-21", "16h00", "Bélgica", "Irã", "G", "SoFi Stadium", "Los Angeles"),
        new JogoSeed("2026-06-21", "22h00", "Nova Zelândia", "Egito", "G", "BC Place", "Vancouver"),
        new JogoSeed("2026-06-27", "00h00", "Egito", "Irã", "G", "Lumen Field", "Seattle"),
        new JogoSeed("2026-06-27", "00h00", "Nova Zelândia", "Bélgica", "G", "BC Place", "Vancouver"),
        // Grupo H
        new JogoSeed("2026-06-15", "13h00", "Espanha", "Cabo Verde", "H", "Mercedes-Benz Stadium", "Atlanta"),
        new JogoSeed("2026-06-15", "19h00", "Arábia Saudita", "Uruguai", "H", "Hard Rock Stadium", "Miami"),
        new JogoSeed("2026-06-21", "13h00", "Espanha", "Arábia Saudita", "H", "Mercedes-Benz Stadium", "Atlanta"),
        new JogoSeed("2026-06-21", "19h00", "Uruguai", "Cabo Verde", "H", "Hard Rock Stadium", "Miami"),
        new JogoSeed("2026-06-26", "21h00", "Cabo Verde", "Arábia Saudita", "H", "NRG Stadium", "Houston"),
        new JogoSeed("2026-06-26", "21h00", "Uruguai", "Espanha", "H", "Estadio Akron", "Guadalajara"),
        // Grupo I
        new JogoSeed("2026-06-16", "16h00", "França", "Senegal", "I", "MetLife Stadium", "Nova Jersey"),
        new JogoSeed("2026-06-16", "19h00", "Iraque", "Noruega", "I", "Gillette Stadium", "Boston"),
        new JogoSeed("2026-06-22", "18h00", "França", "Iraque", "I", "Lincoln Financial Field", "Filadélfia"),
        new JogoSeed("2026-06-22", "21h00", "Noruega", "Senegal", "I", "MetLife Stadium", "Nova Jersey"),
        new JogoSeed("2026-06-26", "16h00", "Noruega", "França", "I", "Gillette Stadium", "Boston"),
        new JogoSeed("2026-06-26", "16h00", "Senegal", "Iraque", "I", "BMO Field", "Toronto"),
        // Grupo J
        new JogoSeed("2026-06-16", "22h00", "Argentina", "Argélia", "J", "GEHA Field at Arrowhead", "Kansas City"),
        new JogoSeed("2026-06-17", "01h00", "Áustria", "Jordânia", "J", "Levi's Stadium", "São Francisco"),
        new JogoSeed("2026-06-22", "14h00", "Argentina", "Áustria", "J", "AT&T Stadium", "Dallas"),
        new JogoSeed("2026-06-23", "00h00", "Jordânia", "Argélia", "J", "Levi's Stadium", "São Francisco"),
        new JogoSeed("2026-06-27", "23h00", "Argélia", "Áustria", "J", "GEHA Field at Arrowhead", "Kansas City"),
        new JogoSeed("2026-06-27", "23h00", "Jordânia", "Argentina", "J", "AT&T Stadium", "Dallas"),
        // Grupo K
        new JogoSeed("2026-06-17", "14h00", "Portugal", "RD Congo", "K", "NRG Stadium", "Houston"),
        new JogoSeed("2026-06-17", "23h00", "Uzbequistão", "Colômbia", "K", "Estádio Azteca", "Cidade do México"),
        new JogoSeed("2026-06-23", "14h00", "Portugal", "Uzbequistão", "K", "NRG Stadium", "Houston"),
        new JogoSeed("2026-06-23", "23h00", "Colômbia", "RD Congo", "K", "Estadio Akron", "Guadalajara"),
        new JogoSeed("2026-06-27", "20h30", "Colômbia", "Portugal", "K", "Hard Rock Stadium", "Miami"),
        new JogoSeed("2026-06-27", "20h30", "RD Congo", "Uzbequistão", "K", "Mercedes-Benz Stadium", "Atlanta"),
        // Grupo L
        new JogoSeed("2026-06-17", "17h00", "Inglaterra", "Croácia", "L", "AT&T Stadium", "Dallas"),
        new JogoSeed("2026-06-17", "20h00", "Gana", "Panamá", "L", "BMO Field", "Toronto"),
        new JogoSeed("2026-06-23", "17h00", "Inglaterra", "Gana", "L", "Gillette Stadium", "Boston"),
        new JogoSeed("2026-06-23", "20h00", "Panamá", "Croácia", "L", "BMO Field", "Toronto"),
        new JogoSeed("2026-06-27", "18h00", "Panamá", "Inglaterra", "L", "MetLife Stadium", "Nova Jersey"),
        new JogoSeed("2026-06-27", "18h00", "Croácia", "Gana", "L", "Lincoln Financial Field", "Filadélfia"),
    };
}

public record SelecaoSeed(string Nome, string Codigo, string Grupo);
public record RankingSeed(string Selecao, int Posicao, decimal Pontos);
public record JogoSeed(string Data, string Hora, string Mandante, string Visitante, string Grupo, string Estadio, string Cidade);
public record PaisSedeSeed(string Pais, string Codigo, int Estadios);

namespace SistemaGestaoConsultasUVV.Services;

/// <summary>
/// Regras de agenda: expediente comercial, grade de horários, fins de semana e
/// feriados nacionais. Usada tanto pela validação do servidor quanto pelo
/// endpoint que alimenta o calendário de disponibilidade.
/// </summary>
public static class Agenda
{
    /// <summary>Primeiro horário de atendimento (08:00).</summary>
    public static readonly TimeOnly Abertura = new(8, 0);

    /// <summary>Fim do expediente (18:00) — o último slot começa 30 min antes.</summary>
    public static readonly TimeOnly Fechamento = new(18, 0);

    public const int SlotMinutos = 30;

    /// <summary>Horários possíveis num dia: 08:00, 08:30, … 17:30.</summary>
    public static IEnumerable<TimeOnly> Slots()
    {
        for (var t = Abertura; t < Fechamento; t = t.AddMinutes(SlotMinutos))
            yield return t;
    }

    /// <summary>Dia útil = segunda a sexta e não é feriado nacional.</summary>
    public static bool DiaUtil(DateOnly dia) =>
        dia.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday) && Feriado(dia) is null;

    /// <summary>Nome do feriado nacional na data, ou <c>null</c> se não for feriado.</summary>
    public static string? Feriado(DateOnly dia)
    {
        foreach (var (d, nome) in FeriadosNacionais(dia.Year))
            if (d == dia) return nome;
        return null;
    }

    /// <summary>Feriados nacionais (fixos + móveis derivados da Páscoa) de um ano.</summary>
    public static IEnumerable<(DateOnly Data, string Nome)> FeriadosNacionais(int ano)
    {
        yield return (new DateOnly(ano, 1, 1), "Confraternização Universal");
        yield return (new DateOnly(ano, 4, 21), "Tiradentes");
        yield return (new DateOnly(ano, 5, 1), "Dia do Trabalho");
        yield return (new DateOnly(ano, 9, 7), "Independência do Brasil");
        yield return (new DateOnly(ano, 10, 12), "Nossa Senhora Aparecida");
        yield return (new DateOnly(ano, 11, 2), "Finados");
        yield return (new DateOnly(ano, 11, 15), "Proclamação da República");
        yield return (new DateOnly(ano, 11, 20), "Consciência Negra");
        yield return (new DateOnly(ano, 12, 25), "Natal");

        var pascoa = Pascoa(ano);
        yield return (pascoa.AddDays(-48), "Carnaval");
        yield return (pascoa.AddDays(-47), "Carnaval");
        yield return (pascoa.AddDays(-2), "Sexta-feira Santa");
        yield return (pascoa.AddDays(60), "Corpus Christi");
    }

    /// <summary>Domingo de Páscoa (algoritmo de Meeus/Butcher).</summary>
    private static DateOnly Pascoa(int ano)
    {
        int a = ano % 19;
        int b = ano / 100;
        int c = ano % 100;
        int d = b / 4;
        int e = b % 4;
        int f = (b + 8) / 25;
        int g = (b - f + 1) / 3;
        int h = (19 * a + b - d - g + 15) % 30;
        int i = c / 4;
        int k = c % 4;
        int l = (32 + 2 * e + 2 * i - h - k) % 7;
        int m = (a + 11 * h + 22 * l) / 451;
        int mes = (h + l - 7 * m + 114) / 31;
        int dia = ((h + l - 7 * m + 114) % 31) + 1;
        return new DateOnly(ano, mes, dia);
    }
}

namespace BarberBoss.Domain.Extensions;

public static class DateExtensions
{
    /// <summary>Segunda-feira da semana que contém a data informada.</summary>
    public static DateOnly StartOfWeek(this DateOnly date)
    {
        var diff = ((int)date.DayOfWeek + 6) % 7; // domingo = 0 -> 6 dias após a segunda
        return date.AddDays(-diff);
    }

    /// <summary>Domingo da semana que contém a data informada.</summary>
    public static DateOnly EndOfWeek(this DateOnly date) => date.StartOfWeek().AddDays(6);
}

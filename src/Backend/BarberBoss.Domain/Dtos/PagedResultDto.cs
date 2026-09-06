namespace BarberBoss.Domain.Dtos;

public class PagedResultDto<T>
{
    public IList<T> Items { get; set; } = [];
    public int TotalItems { get; set; }
}

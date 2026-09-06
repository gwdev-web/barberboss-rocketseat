using BarberBoss.Communication.Requests;

namespace CommonTestUtilities.Requests;

public static class RequestBillingFilterJsonBuilder
{
    public static RequestBillingFilterJson Build(int pageNumber = 1, int pageSize = 20)
        => new() { PageNumber = pageNumber, PageSize = pageSize };
}

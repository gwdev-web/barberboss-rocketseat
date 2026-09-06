using System.Net;

namespace BarberBoss.Exception.ExceptionsBase;

public class NotFoundException : BarberBossException
{
    public NotFoundException(string message) : base(message) { }

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.NotFound;

    public override IList<string> GetErrors() => [Message];
}

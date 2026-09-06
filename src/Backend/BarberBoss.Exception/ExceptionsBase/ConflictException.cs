using System.Net;

namespace BarberBoss.Exception.ExceptionsBase;

public class ConflictException : BarberBossException
{
    public ConflictException(string message) : base(message) { }

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Conflict;

    public override IList<string> GetErrors() => [Message];
}

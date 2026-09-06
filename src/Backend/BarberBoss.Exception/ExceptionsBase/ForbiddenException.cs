using System.Net;

namespace BarberBoss.Exception.ExceptionsBase;

public class ForbiddenException : BarberBossException
{
    public ForbiddenException() : base(ResourceMessagesException.NO_PERMISSION) { }

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Forbidden;

    public override IList<string> GetErrors() => [Message];
}

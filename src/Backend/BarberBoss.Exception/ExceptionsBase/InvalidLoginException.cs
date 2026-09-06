using System.Net;

namespace BarberBoss.Exception.ExceptionsBase;

public class InvalidLoginException : BarberBossException
{
    public InvalidLoginException() : base(ResourceMessagesException.EMAIL_OR_PASSWORD_INVALID) { }

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;

    public override IList<string> GetErrors() => [Message];
}

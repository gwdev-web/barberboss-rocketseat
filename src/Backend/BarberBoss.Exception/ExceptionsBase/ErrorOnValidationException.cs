using System.Net;

namespace BarberBoss.Exception.ExceptionsBase;

public class ErrorOnValidationException : BarberBossException
{
    private readonly IList<string> _errors;

    public ErrorOnValidationException(IList<string> errors) : base(string.Empty) => _errors = errors;

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;

    public override IList<string> GetErrors() => _errors;
}

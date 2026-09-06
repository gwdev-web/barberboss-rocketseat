using System.Net;

namespace BarberBoss.Exception.ExceptionsBase;

public abstract class BarberBossException : SystemException
{
    protected BarberBossException(string message) : base(message) { }

    public abstract HttpStatusCode GetStatusCode();

    public abstract IList<string> GetErrors();
}

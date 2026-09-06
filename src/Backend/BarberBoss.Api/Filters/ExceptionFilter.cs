using BarberBoss.Communication.Responses;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BarberBoss.Api.Filters;

/// <summary>
/// Filtro global de exceções. Traduz as exceções de domínio para o status code
/// correspondente e devolve sempre o mesmo contrato de erro (ResponseErrorJson).
/// </summary>
public class ExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionFilter> _logger;

    public ExceptionFilter(ILogger<ExceptionFilter> logger) => _logger = logger;

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is BarberBossException barberBossException)
            HandleKnownException(context, barberBossException);
        else
            HandleUnknownException(context);
    }

    private static void HandleKnownException(ExceptionContext context, BarberBossException exception)
    {
        context.HttpContext.Response.StatusCode = (int)exception.GetStatusCode();
        context.Result = new ObjectResult(new ResponseErrorJson(exception.GetErrors()));
    }

    private void HandleUnknownException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Erro não tratado ao processar a requisição.");

        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Result = new ObjectResult(new ResponseErrorJson(ResourceMessagesException.UNKNOWN_ERROR));
    }
}

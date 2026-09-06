using BarberBoss.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BarberBoss.Api.Filters;

/// <summary>
/// Faz os erros de binding do ASP.NET (ex.: JSON malformado, enum inválido)
/// saírem no mesmo formato do restante da API.
/// </summary>
public static class InvalidModelStateResponse
{
    public static IActionResult Build(ActionContext context)
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .SelectMany(entry => entry.Value!.Errors)
            .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                ? "Requisição inválida."
                : error.ErrorMessage)
            .Distinct()
            .ToList();

        return new BadRequestObjectResult(new ResponseErrorJson(errors));
    }
}

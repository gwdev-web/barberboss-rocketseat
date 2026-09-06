using BarberBoss.Application.UseCases.Billings.Delete;
using BarberBoss.Application.UseCases.Billings.GetAll;
using BarberBoss.Application.UseCases.Billings.GetById;
using BarberBoss.Application.UseCases.Billings.Register;
using BarberBoss.Application.UseCases.Billings.Summary;
using BarberBoss.Application.UseCases.Billings.Update;
using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BarberBoss.Api.Controllers;

[Route("api/billings")]
[ApiController]
[ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status500InternalServerError)]
public class BillingsController : ControllerBase
{
    /// <summary>Cria um novo faturamento.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisteredBillingJson), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register(
        [FromServices] IRegisterBillingUseCase useCase,
        [FromBody] RequestBillingJson request)
    {
        var response = await useCase.Execute(request);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>Lista faturamentos com filtros, paginação e ordenação.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResponsePagedJson<ResponseShortBillingJson>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromServices] IGetAllBillingsUseCase useCase,
        [FromQuery] RequestBillingFilterJson request)
    {
        var response = await useCase.Execute(request);

        return Ok(response);
    }

    /// <summary>Total do período. Soma apenas os faturamentos com status Pago.</summary>
    /// <remarks>Sem datas, considera a semana corrente (segunda a domingo).</remarks>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ResponseBillingsSummaryJson), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(
        [FromServices] IGetBillingsSummaryUseCase useCase,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate)
    {
        var response = await useCase.Execute(startDate, endDate);

        return Ok(response);
    }

    /// <summary>Obtém um faturamento pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ResponseBillingJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromServices] IGetBillingByIdUseCase useCase,
        [FromRoute] Guid id)
    {
        var response = await useCase.Execute(id);

        return Ok(response);
    }

    /// <summary>Atualiza um faturamento existente.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromServices] IUpdateBillingUseCase useCase,
        [FromRoute] Guid id,
        [FromBody] RequestBillingJson request)
    {
        await useCase.Execute(id, request);

        return NoContent();
    }

    /// <summary>Exclui um faturamento pelo id.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromServices] IDeleteBillingUseCase useCase,
        [FromRoute] Guid id)
    {
        await useCase.Execute(id);

        return NoContent();
    }
}

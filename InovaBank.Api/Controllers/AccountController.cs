using InovaBank.Api.DTOs.Requests;
using InovaBank.Application.Features.Accounts.Commands.OpenAccount;
using InovaBank.Application.Features.Accounts.Queries.GetAccountByCnpj;
using InovaBank.Application.Features.Accounts.Queries.GetAccountById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InovaBank.Api.Controllers;

public sealed class AccountController(IMediator _mediator) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OpenAccountRequest request)
    {
        var command = new OpenAccountCommand(request.Cnpj, request.Agencia, request.ImagemDocumento);
        var result = await _mediator.Send(command);

        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var result = await _mediator.Send(new GetAccountByIdQuery(id));
        return HandleResult(result);
    }

    [HttpGet("cnpj/{cnpj}")]
    public async Task<IActionResult> GetByCnpj(string cnpj)
    {
        var result = await _mediator.Send(new GetAccountByCnpjQuery(cnpj));
        return HandleResult(result);
    }
}

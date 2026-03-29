using InovaBank.Api.DTOs.Requests;
using InovaBank.Application.Features.Accounts.Commands.OpenAccount;
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
}

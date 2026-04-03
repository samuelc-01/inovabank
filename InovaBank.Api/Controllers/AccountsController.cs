using InovaBank.Api.DTOs.Requests;
using InovaBank.Application.Features.Accounts.Commands.ChangeStatus;
using InovaBank.Application.Features.Accounts.Commands.CloseAccount;
using InovaBank.Application.Features.Accounts.Commands.OpenAccount;
using InovaBank.Application.Features.Accounts.Queries.GetAccountByCnpj;
using InovaBank.Application.Features.Accounts.Queries.GetAccountById;
using InovaBank.Application.Features.Accounts.Queries.GetBalance;
using InovaBank.Application.Features.Accounts.Queries.GetStatement;
using InovaBank.Application.Features.Transactions.Commands.Deposit;
using InovaBank.Application.Features.Transactions.Commands.Transfer;
using InovaBank.Application.Features.Transactions.Commands.Withdraw;
using InovaBank.Domain.Entities;
using InovaBank.Domain.Primitives;
using InovaBank.Domain.Queries.ReadModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InovaBank.Api.Controllers;

public sealed class AccountsController(IMediator _mediator) : ApiControllerBase
{
    /// <summary> Solicita a abertura de uma nova conta bancária para empresa. </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] OpenAccountRequest request)
    {
        var command = new OpenAccountCommand(request.Cnpj, request.Agencia, request.ImagemDocumento);
        var result = await _mediator.Send(command);

        return HandleResult(result);
    }

    /// <summary> Obtém os dados cadastrais da conta pelo seu identificador único. </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<Account>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var result = await _mediator.Send(new GetAccountByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary> Obtém os dados cadastrais da conta através do número do CNPJ. </summary>
    [HttpGet("cnpj/{cnpj}")]
    [ProducesResponseType(typeof(ApiResponse<Account>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCnpj(string cnpj)
    {
        var result = await _mediator.Send(new GetAccountByCnpjQuery(cnpj));
        return HandleResult(result);
    }

    /// <summary> Consulta o saldo atual disponível (Read Model no MongoDB). </summary>
    [HttpGet("{id}/balance")]
    [ProducesResponseType(typeof(ApiResponse<BalanceReadModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBalance([FromRoute] string id)
    {
        var result = await _mediator.Send(new GetBalanceQuery(id));

        return HandleResult(result);
    }

    /// <summary> Gera o extrato de transações com filtros de data, tipo e paginação. </summary>
    [HttpGet("{id}/statement")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<StatementReadModel>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatement(
        [FromRoute] string id,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] string? tipo,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 20)
    {
        var query = new GetStatementQuery(id, dataInicio, dataFim, tipo, pagina, tamanhoPagina);
        var result = await _mediator.Send(query);

        return HandleResult(result);
    }

    /// <summary> Altera o status atual da conta (Ativa, Bloqueada ou Encerrada). </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeStatus(string id, [FromBody] ChangeStatusRequest request)
    {
        var result = await _mediator.Send(new ChangeStatusCommand(id, request.Status));
        return HandleResult(result);
    }

    /// <summary> Realiza o encerramento da conta (necessário saldo zerado). </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Close(string id)
    {
        var result = await _mediator.Send(new CloseAccountCommand(id));
        return HandleResult(result);
    }

    /// <summary> Efetua um depósito em conta utilizando chave de idempotência. </summary>
    [HttpPost("{id}/deposit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Deposit(string id, [FromBody] DepositRequest request)
    {
        var command = new DepositCommand(
            id,
            request.IdempotencyKey,
            request.Valor,
            request.Moeda,
            request.Descricao);

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary> Efetua um saque em conta utilizando chave de idempotência. </summary>
    [HttpPost("{id}/withdraw")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Withdraw(string id, [FromBody] WithdrawRequest request)
    {
        var command = new WithdrawCommand(
            id,
            request.IdempotencyKey,
            request.Valor,
            request.Moeda,
            request.Descricao);

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary> Realiza uma transferência entre contas com validação de status e saldo. </summary>
    [HttpPost("{id}/transfer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Transfer(string id, [FromBody] TransferRequest request)
    {
        var command = new TransferCommand(
            id,
            request.IdempotencyKey,
            request.ContaDestinoId,
            request.Valor,
            request.Moeda,
            request.Descricao);

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }
}

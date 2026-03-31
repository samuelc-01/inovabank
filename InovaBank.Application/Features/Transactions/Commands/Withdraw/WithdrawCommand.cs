using InovaBank.Domain.Primitives;
using MediatR;

namespace InovaBank.Application.Features.Transactions.Commands.Withdraw;

public sealed record WithdrawCommand(
    string AccountId,
    string IdempotencyKey,
    decimal Valor,
    string Moeda,
    string Descricao) : IRequest<Result<Unit>>;

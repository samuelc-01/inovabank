using InovaBank.Domain.Primitives;
using MediatR;

namespace InovaBank.Application.Features.Transactions.Commands.Deposit;

public sealed record DepositCommand(
    string AccountId, 
    string IdempotencyKey, 
    decimal Valor, 
    string Moeda, 
    string Descricao) : IRequest<Result<Unit>>;
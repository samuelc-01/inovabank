using InovaBank.Domain.Primitives;
using MediatR;

namespace InovaBank.Application.Features.Transactions.Commands.Transfer;

public sealed record TransferCommand(
    string SourceAccountId,
    string IdempotencyKey,
    string DestinationAccountId,
    decimal Valor,
    string Moeda,
    string Descricao) : IRequest<Result<Unit>>;

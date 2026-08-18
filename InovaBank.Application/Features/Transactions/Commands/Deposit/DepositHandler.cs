using InovaBank.Domain.Events.Transactions;
using InovaBank.Domain.Interfaces;
using InovaBank.Domain.Primitives;
using InovaBank.Infrastructure.Telemetry;
using MediatR;
using MassTransit;

namespace InovaBank.Application.Features.Transactions.Commands.Deposit;

public sealed class DepositHandler(IAccountRepository _repository, IUnitOfWork _unityOfWork, ICacheService _cache, IPublishEndpoint _publishEndpoint) : IRequestHandler<DepositCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DepositCommand request, CancellationToken ct)
    {
        BankingMetrics.DepositsCompleted.Add(1,
            new KeyValuePair<string, object?>("currency", request.Moeda));
        BankingMetrics.TransactionAmount.Record(request.Valor);

        string cacheKey = $"idempotency:deposit:{request.IdempotencyKey}";

        if (await _cache.GetAsync<bool>(cacheKey, ct))
            return Result<Unit>.Success(Unit.Value);

        var guidId = Guid.Parse(request.AccountId);

        var account = await _repository.GetByIdAsync(guidId, ct);
        if (account is null)
        {
            BankingMetrics.TransfersFailed.Add(1);
            return Result<Unit>.Failure("Conta não encontrada.", 404);
        }

        var result = account.Credit(request.Valor, request.Moeda, request.Descricao);
        if (result.IsFailure)
        {
            BankingMetrics.TransfersFailed.Add(1);
            return Result<Unit>.Failure(result.Error!, result.StatusCode);
        }

        var transaction = account.Transactions.Last();

        await _publishEndpoint.Publish(new TransactionCreatedEvent(
            transaction.Id,
            account.Id,
            transaction.Amount,
            transaction.Currency,
            transaction.Type.ToString(),
            transaction.Description,
            transaction.CreatedAt
        ), ct);

        await _unityOfWork.SaveChangesAsync(ct);

        await _cache.SetAsync(cacheKey, true, TimeSpan.FromHours(24), ct);

        return Result<Unit>.Success(Unit.Value);
    }
}

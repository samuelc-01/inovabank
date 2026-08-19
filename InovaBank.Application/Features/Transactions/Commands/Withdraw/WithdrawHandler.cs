using InovaBank.Domain.Interfaces;
using InovaBank.Domain.Primitives;
using InovaBank.Domain.Telemetry;
using MediatR;
using MassTransit;
using InovaBank.Domain.Events.Transactions;

namespace InovaBank.Application.Features.Transactions.Commands.Withdraw;

public sealed class WithdrawHandler(IAccountRepository _repository, IUnitOfWork _unitOfWork, ICacheService _cache, IPublishEndpoint _publishEndpoint) : IRequestHandler<WithdrawCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(WithdrawCommand request, CancellationToken ct)
    {
        var cacheKey = $"idempotency:withdraw:{request.IdempotencyKey}";

        if (await _cache.GetAsync<bool>(cacheKey, ct))
            return Result<Unit>.Success(Unit.Value);

        var guidId = Guid.Parse(request.AccountId);

        var account = await _repository.GetByIdAsync(guidId, ct);
        if (account is null)
        {
            BankingMetrics.WithdrawalsFailed.Add(1);
            return Result<Unit>.Failure("Conta não encontrada.", 404);
        }

        var result = account.Debit(request.Valor, request.Moeda, request.Descricao);

        if (result.IsFailure)
        {
            BankingMetrics.WithdrawalsFailed.Add(1);
            return Result<Unit>.Failure(result.Error!, result.StatusCode);
        }

        var transaction = account.Transactions.Last();

        await _publishEndpoint.Publish(new TransactionCreatedEvent(
            transaction.Id,
            account.Id,
            -transaction.Amount,
            transaction.Currency,
            transaction.Type.ToString(),
            transaction.Description,
            transaction.CreatedAt
        ), ct);

        await _unitOfWork.SaveChangesAsync(ct);

        BankingMetrics.WithdrawalsCompleted.Add(1,
            new KeyValuePair<string, object?>("currency", request.Moeda));
        BankingMetrics.TransactionAmount.Record(request.Valor);

        await _cache.SetAsync(cacheKey, true, TimeSpan.FromHours(24), ct);

        return Result<Unit>.Success(Unit.Value);
    }
}

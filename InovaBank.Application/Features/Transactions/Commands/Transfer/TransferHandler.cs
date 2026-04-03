using InovaBank.Domain.Events.Transactions;
using InovaBank.Domain.Interfaces;
using InovaBank.Domain.Primitives;
using MassTransit;
using MediatR;

namespace InovaBank.Application.Features.Transactions.Commands.Transfer;

public sealed class TransferHandler(IAccountRepository _repository, IUnitOfWork _unityOfWork, ICacheService _cache, IPublishEndpoint _publishEndpoint) : IRequestHandler<TransferCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(TransferCommand request, CancellationToken ct)
    {
        var cacheKey = $"idempotency:transfer:{request.IdempotencyKey}";

        if (await _cache.GetAsync<bool>(cacheKey, ct))
            return Result<Unit>.Success(Unit.Value);

        var sourceGuidId = Guid.Parse(request.SourceAccountId);
        var destGuidId = Guid.Parse(request.DestinationAccountId);

        var source = await _repository.GetByIdAsync(sourceGuidId, ct);
        if (source is null)
            return Result<Unit>.Failure("Conta de origem não encontrada.", 404);

        var destination = await _repository.GetByIdAsync(destGuidId, ct);
        if (destination is null)
            return Result<Unit>.Failure("Conta de destino não encontrada.", 404);

        if (!source.CanPerformTransactions || !destination.CanPerformTransactions)
            return Result<Unit>.Failure("Ambas as contas devem estar ativas para realizar transferências.", 422);

        var debitResult = source.Debit(request.Valor, request.Moeda, $"Transf. Enviada: {request.Descricao}");
        if (!debitResult.IsSuccess)
            return Result<Unit>.Failure(debitResult.Error!, debitResult.StatusCode);

        var creditResult = destination.Credit(request.Valor, request.Moeda, $"Transf. Recebida: {request.Descricao}");
        if (!creditResult.IsSuccess)
            return Result<Unit>.Failure(creditResult.Error!, creditResult.StatusCode);

        var transaction = source.Transactions.Last();

        await _publishEndpoint.Publish(new TransferCreatedEvent(
            transaction.Id,
            source.Id,
            destination.Id,
            request.Valor,
            request.Moeda,
            request.Descricao,
            transaction.CreatedAt
        ), ct);

        await _unityOfWork.SaveChangesAsync(ct);

        await _cache.SetAsync(cacheKey, true, TimeSpan.FromHours(24), ct);

        return Result<Unit>.Success(Unit.Value);
    }
}

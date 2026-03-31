using InovaBank.Domain.Interfaces;
using InovaBank.Domain.Primitives;
using MediatR;

namespace InovaBank.Application.Features.Transactions.Commands.Withdraw;

public sealed class WithdrawHandler(
    IAccountRepository _repository,
    IUnitOfWork _unitOfWork,
    ICacheService _cache) : IRequestHandler<WithdrawCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(WithdrawCommand request, CancellationToken ct)
    {
        var cacheKey = $"idempotency:withdraw:{request.IdempotencyKey}";

        if (await _cache.GetAsync<bool>(cacheKey, ct))
            return Result<Unit>.Success(Unit.Value);

        var guidId = Guid.Parse(request.AccountId);

        var account = await _repository.GetByIdAsync(guidId, ct);
        if (account is null)
            return Result<Unit>.Failure("Conta não encontrada.", 404);

        var result = account.Debit(request.Valor, request.Moeda, request.Descricao);

        if (result.IsFailure)
            return Result<Unit>.Failure(result.Error!, result.StatusCode);

        await _unitOfWork.SaveChangesAsync(ct);

        await _cache.SetAsync(cacheKey, true, TimeSpan.FromHours(24), ct);

        return Result<Unit>.Success(Unit.Value);
    }
}

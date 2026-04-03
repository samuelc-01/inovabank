using InovaBank.Domain.Interfaces;
using InovaBank.Domain.Primitives;
using InovaBank.Domain.Queries.ReadModels;
using MediatR;

namespace InovaBank.Application.Features.Accounts.Queries.GetBalance;

public sealed class GetBalanceHandler(IAccountReadRepository _readRepository)
    : IRequestHandler<GetBalanceQuery, Result<BalanceReadModel>>
{
    public async Task<Result<BalanceReadModel>> Handle(GetBalanceQuery request, CancellationToken ct)
    {
        var guidId = Guid.Parse(request.Id);

        var balance = await _readRepository.GetBalanceAsync(guidId, ct);

        if (balance is null)
            return Result<BalanceReadModel>.Failure("Saldo não encontrado para esta conta.", 404);

        return Result<BalanceReadModel>.Success(balance);
    }
}

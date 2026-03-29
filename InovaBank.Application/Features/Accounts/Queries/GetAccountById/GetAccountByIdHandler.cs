using InovaBank.Application.Features.Accounts.Queries.Common;
using InovaBank.Domain.Interfaces;
using InovaBank.Domain.Primitives;
using MediatR;

namespace InovaBank.Application.Features.Accounts.Queries.GetAccountById;

public sealed class GetAccountByIdHandler(IAccountRepository _repository) : IRequestHandler<GetAccountByIdQuery, Result<AccountResponse>>
{
    public async Task<Result<AccountResponse>> Handle(GetAccountByIdQuery request, CancellationToken ct)
    {
        var guidId = Guid.Parse(request.Id);
        var account = await _repository.GetByIdAsync(guidId, ct);

        if (account is null)
            return Result<AccountResponse>.Failure("Conta não encontrada.", 404);

        return Result<AccountResponse>.Success(account.ToResponse());
    }
}

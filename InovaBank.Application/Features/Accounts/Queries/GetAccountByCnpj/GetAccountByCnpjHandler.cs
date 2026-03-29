using InovaBank.Application.Features.Accounts.Queries.Common;
using InovaBank.Domain.Interfaces;
using InovaBank.Domain.Primitives;
using InovaBank.Domain.ValueObjects;
using MediatR;

namespace InovaBank.Application.Features.Accounts.Queries.GetAccountByCnpj;

public sealed class GetAccountByCnpjHandler(IAccountRepository _repository) : IRequestHandler<GetAccountByCnpjQuery, Result<AccountResponse>>
{
    public async Task<Result<AccountResponse>> Handle(GetAccountByCnpjQuery request, CancellationToken ct)
    {
        if (!Cnpj.IsValid(request.Cnpj))
            return Result<AccountResponse>.Failure("CNPJ inválido.", 400);

        var account = await _repository.GetByCnpjAsync(new Cnpj(request.Cnpj), ct);

        if (account is null)
            return Result<AccountResponse>.Failure("Conta não encontrada.", 404);

        return Result<AccountResponse>.Success(account.ToResponse());
    }
}

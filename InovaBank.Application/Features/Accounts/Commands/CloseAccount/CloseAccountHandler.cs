using InovaBank.Domain.Interfaces;
using InovaBank.Domain.Primitives;
using MediatR;

namespace InovaBank.Application.Features.Accounts.Commands.CloseAccount;

public sealed class CloseAccountHandler(IAccountRepository _repository, IUnitOfWork _unityOfWork) : IRequestHandler<CloseAccountCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(CloseAccountCommand request, CancellationToken ct)
    {
        var guidId = Guid.Parse(request.Id);

        var account = await _repository.GetByIdAsync(guidId, ct);
        if (account is null) return Result<Unit>.Failure("Conta não encontrada.", 404);

        if (account.Balance != 0)
            return Result<Unit>.Failure("A conta só pode ser encerrada se o saldo for zero.", 400);

        var domainResult = account.Close();

        if (domainResult.IsFailure)
            return Result<Unit>.Failure(domainResult.Error!, 400);

        await _unityOfWork.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}

using InovaBank.Domain.Enums;
using InovaBank.Domain.Interfaces;
using InovaBank.Domain.Primitives;
using MediatR;

namespace InovaBank.Application.Features.Accounts.Commands.ChangeStatus;

public sealed class ChangeStatusHandler(IAccountRepository _repository, IUnitOfWork _unityOfWork) : IRequestHandler<ChangeStatusCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ChangeStatusCommand request, CancellationToken ct)
    {
        var guidId = Guid.Parse(request.Id);

        var account = await _repository.GetByIdAsync(guidId, ct);
        if (account is null) return Result<Unit>.Failure("Conta não encontrada.", 404);

        if (!Enum.TryParse<AccountStatus>(request.Status, true, out var newStatus))
            return Result<Unit>.Failure("Status inválido.", 422);

        var domainResult = account.ChangeStatus(newStatus);

        if (domainResult.IsFailure)
            return Result<Unit>.Failure(domainResult.Error!, domainResult.StatusCode);

        await _unityOfWork.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}

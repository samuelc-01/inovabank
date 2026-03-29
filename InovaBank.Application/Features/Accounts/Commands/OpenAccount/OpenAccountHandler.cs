using InovaBank.Domain.Entities;
using InovaBank.Domain.Interfaces;
using InovaBank.Domain.Primitives;
using InovaBank.Domain.ValueObjects;
using MediatR;

namespace InovaBank.Application.Features.Accounts.Commands.OpenAccount;

public sealed class OpenAccountHandler(
    IAccountRepository _repository,
    IReceitaWsService _receitaService,
    IUnitOfWork _unitOfWork) : IRequestHandler<OpenAccountCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(OpenAccountCommand request, CancellationToken ct)
    {
        var cnpj = new Cnpj(request.Cnpj);
        if (await _repository.ExistsCnpjAsync(cnpj, ct))
            return Result<Guid>.Failure("Já existe uma conta aberta para este CNPJ.", 409);

        var receitaResult = await _receitaService.GetCompanyByCnpjAsync(cnpj, ct);
        if (receitaResult.IsFailure)
            return Result<Guid>.Failure(receitaResult.Error!, receitaResult.StatusCode);

        var account = new Account(
            cnpj,
            receitaResult.Value!.RazaoSocial,
            request.Agencia);

        await _repository.AddAsync(account, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Created(account.Id);
    }
}

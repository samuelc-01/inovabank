using FluentValidation;

namespace InovaBank.Application.Features.Accounts.Queries.GetBalance;

public sealed class GetBalanceValidator : AbstractValidator<GetBalanceQuery>
{
    public GetBalanceValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID é obrigatório.")
            .Must(BeAValidGuid).WithMessage("Formato de ID inválido.");
    }

    private bool BeAValidGuid(string id) => Guid.TryParse(id, out _);
}

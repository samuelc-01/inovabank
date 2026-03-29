using FluentValidation;

namespace InovaBank.Application.Features.Accounts.Queries.GetAccountById;

public sealed class GetAccountByIdValidator : AbstractValidator<GetAccountByIdQuery>
{
    public GetAccountByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID é obrigatório.")
            .Must(BeAValidGuid).WithMessage("O formato do ID fornecido é inválido.");
    }

    private bool BeAValidGuid(string id) => Guid.TryParse(id, out _);
}

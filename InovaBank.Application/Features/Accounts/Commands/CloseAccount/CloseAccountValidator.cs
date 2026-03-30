using FluentValidation;

namespace InovaBank.Application.Features.Accounts.Commands.CloseAccount;

public sealed class CloseAccountValidator : AbstractValidator<CloseAccountCommand>
{
    public CloseAccountValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID é obrigatório como parâmetro.")
            .Must(BeAValidGuid).WithMessage("O formato do ID fornecido é inválido.");
    }

    private bool BeAValidGuid(string id) => Guid.TryParse(id, out _);
}

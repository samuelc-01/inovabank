using FluentValidation;

namespace InovaBank.Application.Features.Transactions.Commands.Withdraw;

public sealed class WithdrawValidator : AbstractValidator<WithdrawCommand>
{
    public WithdrawValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("A identificação da conta de origem é obrigatória.")
            .Must(BeAValidGuid).WithMessage("O formato da conta de origem é inválido");

        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("O valor do saque deve ser maior que zero.");

        RuleFor(x => x.Moeda)
            .Equal("BRL").WithMessage("No momento, apenas a moeda 'BRL' é suportada.");

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty().WithMessage("A chave de idempotência é obrigatória para esta operação.");
    }

    private bool BeAValidGuid(string id) => Guid.TryParse(id, out _);
}

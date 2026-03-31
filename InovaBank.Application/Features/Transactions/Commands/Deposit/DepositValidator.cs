using FluentValidation;

namespace InovaBank.Application.Features.Transactions.Commands.Deposit;

public sealed class DepositValidator : AbstractValidator<DepositCommand>
{
    public DepositValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("O ID é obrigatório como parâmetro.")
            .Must(BeAValidGuid).WithMessage("O formato do ID fornecido é inválido.");

        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("O valor do depósito deve ser maior que zero.");

        RuleFor(x => x.Moeda)
            .Equal("BRL").WithMessage("No momento, apenas a moeda 'BRL' é suportada.");

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty().WithMessage("A chave de idempotência (idempotencyKey) é obrigatória.");

        RuleFor(x => x.Descricao)
            .MaximumLength(100).WithMessage("A descrição deve ter no máximo 100 caracteres.");
    }

    private bool BeAValidGuid(string id) => Guid.TryParse(id, out _);
}

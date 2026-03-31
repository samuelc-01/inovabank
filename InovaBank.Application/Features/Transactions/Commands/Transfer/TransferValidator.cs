using FluentValidation;

namespace InovaBank.Application.Features.Transactions.Commands.Transfer;

public sealed class TransferValidator : AbstractValidator<TransferCommand>
{
    public TransferValidator()
    {
        RuleFor(x => x.SourceAccountId)
            .NotEmpty().WithMessage("A conta de origem é obrigatória.")
            .Must(BeAValidGuid).WithMessage("O formato da conta de origem é inválido.");

        RuleFor(x => x.DestinationAccountId)
            .NotEmpty().WithMessage("A conta de destino é obrigatória.")
            .Must(BeAValidGuid).WithMessage("O formato da conta de destino é inválido.")
            .NotEqual(x => x.SourceAccountId)
            .WithMessage("A conta de destino não pode ser igual à conta de origem.");

        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("O valor da transferência deve ser maior que zero.");

        RuleFor(x => x.Moeda)
            .Equal("BRL").WithMessage("No momento, apenas a moeda 'BRL' é suportada.");

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty().WithMessage("A chave de idempotência é obrigatória para esta operação.");
    }

    private bool BeAValidGuid(string id) => Guid.TryParse(id, out _);
}

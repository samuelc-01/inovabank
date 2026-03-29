using FluentValidation;
using InovaBank.Domain.ValueObjects;

namespace InovaBank.Application.Features.Accounts.Commands.OpenAccount;

public sealed class OpenAccountValidator : AbstractValidator<OpenAccountCommand>
{
    public OpenAccountValidator()
    {
        RuleFor(x => x.Cnpj)
            .NotEmpty().WithMessage("O CNPJ é obrigatório.")
            .Must(Cnpj.IsValid).WithMessage("CNPJ em formato inválido.");

        RuleFor(x => x.Agencia)
            .NotEmpty().WithMessage("A agência é obrigatória.")
            .Length(4).WithMessage("A agência deve ter 4 dígitos.");

        RuleFor(x => x.ImagemDocumento)
            .NotEmpty().WithMessage("A imagem do documento em Base64 é obrigatória.")
            .Must(BeAValidBase64).WithMessage("A imagem deve estar em um formato Base64 válido.");
    }

    private bool BeAValidBase64(string base64)
    {
        if (string.IsNullOrWhiteSpace(base64)) return false;

        Span<byte> buffer = new(new byte[base64.Length]);
        return Convert.TryFromBase64String(base64, buffer, out _);
    }
}

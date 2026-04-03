using FluentValidation;

namespace InovaBank.Application.Features.Accounts.Queries.GetStatement;

public sealed class GetStatementValidator : AbstractValidator<GetStatementQuery>
{
    public GetStatementValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID é obrigatório como parâmetro.")
            .Must(BeAValidGuid).WithMessage("O formato do ID fornecido é inválido.");

        RuleFor(x => x.TamanhoPagina)
            .InclusiveBetween(1, 100).WithMessage("O tamanho da página deve ser entre 1 e 100.");
    }

    private bool BeAValidGuid(string id) => Guid.TryParse(id, out _);
}

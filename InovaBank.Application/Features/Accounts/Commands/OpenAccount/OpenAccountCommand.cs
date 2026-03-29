using InovaBank.Domain.Primitives;
using MediatR;

namespace InovaBank.Application.Features.Accounts.Commands.OpenAccount;

public sealed record OpenAccountCommand(
    string Cnpj,
    string Agencia,
    string ImagemDocumento) : IRequest<Result<Guid>>;

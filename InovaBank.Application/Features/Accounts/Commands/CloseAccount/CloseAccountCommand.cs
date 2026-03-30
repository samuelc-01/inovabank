using InovaBank.Domain.Primitives;
using MediatR;

namespace InovaBank.Application.Features.Accounts.Commands.CloseAccount;

public sealed record CloseAccountCommand(string Id) : IRequest<Result<Unit>>;

using InovaBank.Domain.Primitives;
using MediatR;

namespace InovaBank.Application.Features.Accounts.Commands.ChangeStatus;

public sealed record ChangeStatusCommand(string Id, string Status) : IRequest<Result<Unit>>;

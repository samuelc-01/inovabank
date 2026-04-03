using InovaBank.Domain.Primitives;
using InovaBank.Domain.Queries.ReadModels;
using MediatR;

namespace InovaBank.Application.Features.Accounts.Queries.GetBalance;

public sealed record GetBalanceQuery(string Id) : IRequest<Result<BalanceReadModel>>;

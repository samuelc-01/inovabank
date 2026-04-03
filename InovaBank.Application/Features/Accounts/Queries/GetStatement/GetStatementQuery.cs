using InovaBank.Domain.Primitives;
using InovaBank.Domain.Queries.ReadModels;
using MediatR;

namespace InovaBank.Application.Features.Accounts.Queries.GetStatement;

public sealed record GetStatementQuery(
    string Id,
    DateTime? DataInicio,
    DateTime? DataFim,
    string? Tipo,
    int Pagina,
    int TamanhoPagina) : IRequest<Result<IEnumerable<StatementReadModel>>>;

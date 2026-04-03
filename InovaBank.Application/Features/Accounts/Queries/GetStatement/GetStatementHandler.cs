using InovaBank.Domain.Interfaces;
using InovaBank.Domain.Primitives;
using InovaBank.Domain.Queries.ReadModels;
using MediatR;

namespace InovaBank.Application.Features.Accounts.Queries.GetStatement;

public sealed class GetStatementHandler(IAccountReadRepository _readRepository) : IRequestHandler<GetStatementQuery, Result<PagedResult<StatementReadModel>>>
{
    public async Task<Result<PagedResult<StatementReadModel>>> Handle(GetStatementQuery request, CancellationToken ct)
    {
        var guidId = Guid.Parse(request.Id);

        DateTime? dataInicioUtc = null;
        DateTime? dataFimUtc = null;

        if (request.DataInicio.HasValue)
            dataInicioUtc = DateTime.SpecifyKind(request.DataInicio.Value.Date, DateTimeKind.Utc);

        if (request.DataFim.HasValue)
            dataFimUtc = DateTime.SpecifyKind(request.DataFim.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

        var skip = (request.Pagina - 1) * request.TamanhoPagina;

        var pagedResults = await _readRepository.GetStatementAsync(
            guidId,
            dataInicioUtc,
            dataFimUtc,
            request.Tipo,
            skip,
            request.TamanhoPagina,
            ct);

        return Result<PagedResult<StatementReadModel>>.Success(pagedResults);
    }
}

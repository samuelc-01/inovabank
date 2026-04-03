using InovaBank.Domain.Queries.ReadModels;
using InovaBank.Domain.Primitives;

namespace InovaBank.Domain.Interfaces;

public interface IAccountReadRepository
{
    Task<PagedResult<StatementReadModel>> GetStatementAsync(
        Guid accountId,
        DateTime? start,
        DateTime? end,
        string? type,
        int skip,
        int take,
        CancellationToken ct);

    Task<BalanceReadModel?> GetBalanceAsync(Guid accountId, CancellationToken ct);
}

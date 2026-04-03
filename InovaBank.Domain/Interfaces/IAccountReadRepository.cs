using InovaBank.Domain.Queries.ReadModels;

namespace InovaBank.Domain.Interfaces;

public interface IAccountReadRepository
{
    Task<IEnumerable<StatementReadModel>> GetStatementAsync(
        Guid accountId,
        DateTime? start,
        DateTime? end,
        string? type,
        int skip,
        int take,
        CancellationToken ct);

    Task<BalanceReadModel?> GetBalanceAsync(Guid accountId, CancellationToken ct);
}

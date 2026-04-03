using InovaBank.Domain.Interfaces;
using InovaBank.Domain.Queries.ReadModels;
using InovaBank.Infrastructure.Persistence.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InovaBank.Infrastructure.Persistence.Repositories;

public sealed class AccountReadRepository(MongoContext _context) : IAccountReadRepository
{
    public async Task<IEnumerable<StatementReadModel>> GetStatementAsync(
        Guid accountId, DateTime? start, DateTime? end, string? type, int skip, int take, CancellationToken ct)
    {
        var collection = _context.GetCollection<BsonDocument>("Statements");
        var builder = Builders<BsonDocument>.Filter;
        var filter = builder.Eq("AccountId", accountId);

        if (start.HasValue) filter &= builder.Gte("CreatedAt", start.Value);
        if (end.HasValue) filter &= builder.Lte("CreatedAt", end.Value);
        if (!string.IsNullOrEmpty(type)) filter &= builder.Eq("Type", type);

        var docs = await collection.Find(filter)
            .SortByDescending(x => x["CreatedAt"])
            .Skip(skip)
            .Limit(take)
            .ToListAsync(ct);

        return docs.Select(d => new StatementReadModel(
            d["TransactionId"].AsGuid,
            d["Amount"].ToDecimal(),
            d["Type"].AsString,
            d["Description"].AsString,
            d["CreatedAt"].ToUniversalTime()));
    }

    public async Task<BalanceReadModel?> GetBalanceAsync(Guid accountId, CancellationToken ct)
    {
        var collection = _context.GetCollection<BsonDocument>("Balances");
        var filter = Builders<BsonDocument>.Filter.Eq("AccountId", accountId);

        var doc = await collection.Find(filter).FirstOrDefaultAsync(ct);

        if (doc is null) return null;

        return new BalanceReadModel(
            accountId,
            doc["CurrentBalance"].ToDecimal());
    }
}

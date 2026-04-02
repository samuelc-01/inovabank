using InovaBank.Domain.Events.Transactions;
using InovaBank.Worker.Infrastructure;
using MassTransit;
using MongoDB.Driver;

namespace InovaBank.Worker.Consumers;

public sealed class TransferCreatedConsumer(MongoContext _mongoContext) : IConsumer<TransferCreatedEvent>
{
    public async Task Consume(ConsumeContext<TransferCreatedEvent> context)
    {
        var @event = context.Message;
        var statements = _mongoContext.GetCollection<dynamic>("Statements");
        var balances = _mongoContext.GetCollection<dynamic>("Balances");

        var debitDoc = new
        {
            @event.TransactionId,
            AccountId = @event.SourceAccountId,
            Amount = -@event.Amount,
            Type = "TransferenciaEnviada",
            @event.Description,
            @event.CreatedAt
        };

        var creditDoc = new
        {
            TransactionId = Guid.NewGuid(),
            AccountId = @event.DestinationAccountId,
            @event.Amount,
            Type = "TransferenciaRecebida",
            @event.Description,
            @event.CreatedAt
        };

        await statements.InsertManyAsync([debitDoc, creditDoc]);

        await UpdateBalance(@event.SourceAccountId, -@event.Amount, balances);
        await UpdateBalance(@event.DestinationAccountId, @event.Amount, balances);
    }

    private Task UpdateBalance(Guid accId, decimal val, IMongoCollection<dynamic> col) =>
        col.UpdateOneAsync(
            Builders<dynamic>.Filter.Eq("AccountId", accId),
            Builders<dynamic>.Update.Inc("CurrentBalance", val),
            new UpdateOptions { IsUpsert = true });
}

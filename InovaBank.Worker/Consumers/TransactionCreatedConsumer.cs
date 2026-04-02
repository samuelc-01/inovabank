using MassTransit;
using MongoDB.Driver;
using InovaBank.Domain.Events.Transactions;
using InovaBank.Worker.Infrastructure;

namespace InovaBank.Worker.Consumers;

public sealed class TransactionCreatedConsumer(MongoContext _mongoContext, ILogger<TransactionCreatedConsumer> _logger) : IConsumer<TransactionCreatedEvent>
{
    public async Task Consume(ConsumeContext<TransactionCreatedEvent> context)
    {
        var @event = context.Message;

        var statements = _mongoContext.GetCollection<dynamic>("Statements");
        var balances = _mongoContext.GetCollection<dynamic>("Balances");

        await statements.InsertOneAsync(new
        {
            @event.TransactionId,
            @event.AccountId,
            @event.Amount,
            @event.Type,
            @event.Description,
            @event.CreatedAt
        });

        var filter = Builders<dynamic>.Filter.Eq("AccountId", @event.AccountId);
        var update = Builders<dynamic>.Update.Inc("CurrentBalance", @event.Amount);

        await balances.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }
}

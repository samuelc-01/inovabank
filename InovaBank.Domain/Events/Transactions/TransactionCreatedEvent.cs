namespace InovaBank.Domain.Events.Transactions;

public sealed record TransactionCreatedEvent(
    Guid TransactionId,
    Guid AccountId,
    decimal Amount,
    string Currency,
    string Type,
    string Description,
    DateTime CreatedAt);

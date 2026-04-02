namespace InovaBank.Domain.Events.Transactions;

public sealed record TransferCreatedEvent(
    Guid TransactionId,
    Guid SourceAccountId,
    Guid DestinationAccountId,
    decimal Amount,
    string Currency,
    string Description,
    DateTime CreatedAt);

using InovaBank.Domain.Enums;

namespace InovaBank.Domain.Entities;

public class Transaction : Entity
{
    public Guid AccountId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public TransactionType Type { get; private set; }
    public string Description { get; private set; }

    public Transaction(Guid accountId, decimal amount, string currency, TransactionType type, string description)
    {
        Id = Guid.NewGuid();
        AccountId = accountId;
        Amount = amount;
        Currency = currency;
        Type = type;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    private Transaction() { }
}

namespace InovaBank.Domain.Queries.ReadModels;

public sealed record BalanceReadModel(Guid AccountId, decimal Saldo);

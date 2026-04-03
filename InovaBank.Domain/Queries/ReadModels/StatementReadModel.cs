namespace InovaBank.Domain.Queries.ReadModels;

public sealed record StatementReadModel(
    Guid Id,
    decimal Valor,
    string Tipo,
    string Descricao,
    DateTime Data);

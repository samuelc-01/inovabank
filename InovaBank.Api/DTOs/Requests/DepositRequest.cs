namespace InovaBank.Api.DTOs.Requests;

public sealed record DepositRequest(
    string IdempotencyKey,
    decimal Valor,
    string Moeda,
    string Descricao);

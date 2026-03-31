namespace InovaBank.Api.DTOs.Requests;

public sealed record WithdrawRequest(
    string IdempotencyKey,
    decimal Valor,
    string Moeda,
    string Descricao);

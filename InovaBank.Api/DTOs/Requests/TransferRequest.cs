namespace InovaBank.Api.DTOs.Requests;

public sealed record TransferRequest(
    string IdempotencyKey,
    string ContaDestinoId,
    decimal Valor,
    string Moeda,
    string Descricao);

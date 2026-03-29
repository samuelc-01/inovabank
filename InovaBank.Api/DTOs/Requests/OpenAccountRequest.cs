namespace InovaBank.Api.DTOs.Requests;

public sealed record OpenAccountRequest(
    string Cnpj,
    string Agencia,
    string ImagemDocumento);

namespace InovaBank.Domain.Events.Accounts;

public sealed record AccountCreatedEvent(
    Guid Id,
    string Cnpj,
    string Agencia,
    string RazaoSocial);

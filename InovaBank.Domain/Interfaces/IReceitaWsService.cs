using InovaBank.Domain.Primitives;
using InovaBank.Domain.ValueObjects;

namespace InovaBank.Domain.Interfaces;

public interface IReceitaWsService
{
    Task<Result<ReceitaWsResult>> GetCompanyByCnpjAsync(Cnpj cnpj, CancellationToken ct);
}

public sealed record ReceitaWsResult(string RazaoSocial, string Status);

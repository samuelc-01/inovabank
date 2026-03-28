using System.Net.Http.Json;
using InovaBank.Domain.Interfaces;
using InovaBank.Domain.Primitives;
using InovaBank.Domain.ValueObjects;

namespace InovaBank.Infrastructure.Services.ReceitaWs;

public sealed class ReceitaWsService(HttpClient httpClient) : IReceitaWsService
{
    public async Task<Result<ReceitaWsResult>> GetCompanyByCnpjAsync(Cnpj cnpj, CancellationToken ct)
    {
        try
        {
            var response = await httpClient.GetAsync($"v1/cnpj/{cnpj.Number}", ct);

            if (!response.IsSuccessStatusCode)
                return Result<ReceitaWsResult>.Failure("Erro ao consultar ReceitaWS.", (int)response.StatusCode);

            var data = await response.Content.ReadFromJsonAsync<ReceitaWsDto>(cancellationToken: ct);

            if (data == null || data.Status == "ERROR")
                return Result<ReceitaWsResult>.Failure(data?.Message ?? "CNPJ não encontrado.", 404);

            return Result<ReceitaWsResult>.Success(new ReceitaWsResult(data.Nome, data.Status));
        }
        catch (Exception)
        {
            return Result<ReceitaWsResult>.Failure("Serviço da ReceitaWS indisponível.", 503);
        }
    }
}

internal sealed record ReceitaWsDto(string Nome, string Status, string Message);

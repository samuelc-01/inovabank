using System.Net.Http.Json;
using InovaBank.Domain.Interfaces;
using InovaBank.Domain.Primitives;
using InovaBank.Domain.ValueObjects;

namespace InovaBank.Infrastructure.Services.ReceitaWs;

public sealed class ReceitaWsService(HttpClient httpClient, ICacheService cache) : IReceitaWsService
{
    public async Task<Result<ReceitaWsResult>> GetCompanyByCnpjAsync(Cnpj cnpj, CancellationToken ct)
    {
        var cacheKey = $"cnpj:{cnpj.Number}";

        var cachedResult = await cache.GetAsync<ReceitaWsResult>(cacheKey, ct);
        if (cachedResult is not null) return Result<ReceitaWsResult>.Success(cachedResult);

        try
        {
            var response = await httpClient.GetAsync($"v1/cnpj/{cnpj.Number}", ct);

            if (!response.IsSuccessStatusCode)
                return Result<ReceitaWsResult>.Failure("Erro ao consultar ReceitaWS.", (int)response.StatusCode);

            var data = await response.Content.ReadFromJsonAsync<ReceitaWsDto>(cancellationToken: ct);

            if (data == null || data.Status == "ERROR")
                return Result<ReceitaWsResult>.Failure(data?.Message ?? "CNPJ não encontrado.", 404);

            var result = new ReceitaWsResult(data.Nome, data.Status);

            await cache.SetAsync(cacheKey, result, TimeSpan.FromHours(24), ct);

            return Result<ReceitaWsResult>.Success(result);
        }
        catch (Exception)
        {
            return Result<ReceitaWsResult>.Failure("Serviço da ReceitaWS indisponível.", 503);
        }
    }
}

internal sealed record ReceitaWsDto(string Nome, string Status, string Message);

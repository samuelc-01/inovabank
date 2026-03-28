using InovaBank.Domain.Interfaces;
using InovaBank.Infrastructure.Persistence;
using InovaBank.Infrastructure.Persistence.Repositories;
using InovaBank.Infrastructure.Services.ReceitaWs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace InovaBank.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<InovaBankDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.AddHttpClient<IReceitaWsService, ReceitaWsService>(client =>
        {
            client.BaseAddress = new Uri("https://receitaws.com.br/");
        }).
        AddTransientHttpErrorPolicy(p =>
              p.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<InovaBankDbContext>());

        services.AddScoped<IAccountRepository, AccountRepository>();

        return services;
    }
}

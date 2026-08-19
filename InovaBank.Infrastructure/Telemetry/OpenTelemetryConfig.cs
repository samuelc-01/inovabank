using System.Diagnostics;
using System.Reflection;
using InovaBank.Domain.Telemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace InovaBank.Infrastructure.Telemetry;

public static class OpenTelemetryConfig
{
    private static readonly ActivitySource ActivitySource = new("InovaBank");

    public static IServiceCollection AddOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName)
    {
        var otelEndpoint = configuration.GetValue<string>("OpenTelemetry:Endpoint")
                           ?? "http://localhost:4317";

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddSource("InovaBank")
                    .AddSource("MassTransit")
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otelEndpoint);
                    });
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter(BankingMetrics.MeterName)
                    .AddMeter("MassTransit")
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otelEndpoint);
                    });
            });

        services.AddSingleton(ActivitySource);

        return services;
    }
}

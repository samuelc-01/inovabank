using InovaBank.Infrastructure.Persistence.MongoDb;
using InovaBank.Worker.Consumers;
using MassTransit;
using InovaBank.Infrastructure.Telemetry;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ServiceName", "InovaBank.Worker")
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} " +
        "[TraceId:{TraceId} SpanId:{SpanId}]{NewLine}{Exception}")
    .CreateLogger();

builder.Services.AddSingleton<MongoContext>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TransactionCreatedConsumer>();
    x.AddConsumer<TransferCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddOpenTelemetry(builder.Configuration, "InovaBank.Worker");

builder.Services.AddSerilog(dispose: true);

var host = builder.Build();
host.Run();

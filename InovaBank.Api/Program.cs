using FluentValidation;
using InovaBank.Application.Behaviors;
using InovaBank.Infrastructure;
using InovaBank.Infrastructure.Persistence;
using MassTransit;
using Microsoft.OpenApi;
using InovaBank.Infrastructure.Telemetry;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Postgres")!;
var redisConnection = builder.Configuration.GetConnectionString("Redis")!;
var rabbitConnection = builder.Configuration.GetConnectionString("RabbitMq")!;

builder.Services.AddHealthChecks()
    .AddPostgres(connectionString, name: "Postgres")
    .AddRedis(redisConnection, name: "Redis")
    .AddRabbitMq(rabbitConnection, name: "RabbitMq");



builder.Services.AddValidatorsFromAssembly(typeof(InovaBank.Application.AssemblyReference).Assembly);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(InovaBank.Application.AssemblyReference).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<InovaBankDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox();
        o.DisableInboxCleanupService();
    });

    x.AddConfigureEndpointsCallback((context, name, cfg) =>
    {
        cfg.UseEntityFrameworkOutbox<InovaBankDbContext>(context);
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "InovaBank API",
        Version = "v1",
        Description = "Plataforma Bancária com CQRS e Event-Driven Architecture."
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddOpenTelemetry(builder.Configuration, "InovaBank.Api");

builder.Host.UseSerilog((context, config) =>
{
    config
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("ServiceName", "InovaBank.Api")
        .WriteTo.Console(outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} " +
            "[TraceId:{TraceId} SpanId:{SpanId}]{NewLine}{Exception}");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description,
                exception = e.Value.Exception?.Message
            })
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});
app.UseHttpsRedirection();

app.MapControllers();

app.Run();

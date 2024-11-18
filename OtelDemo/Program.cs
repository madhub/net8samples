using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection.PortableExecutable;

var appBuilder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Clear default logging providers used by WebApplication host.
appBuilder.Logging.ClearProviders();

// Configure OpenTelemetry logging, metrics, & tracing with auto-start using the
// AddOpenTelemetry extension from OpenTelemetry.Extensions.Hosting.
appBuilder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r
        .AddService(
            serviceName: appBuilder.Configuration.GetValue("ServiceName", defaultValue: "otel-test")!,
            serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown",
            serviceInstanceId: Environment.MachineName))
    .WithTracing(builder =>
    {
        // Tracing

        // Ensure the TracerProvider subscribes to any custom ActivitySources.
        builder
//            .AddSource(Instrumentation.ActivitySourceName)
            .SetSampler(new AlwaysOnSampler())
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation();

        // Use IConfiguration binding for AspNetCore instrumentation options.
        appBuilder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(appBuilder.Configuration.GetSection("AspNetCoreInstrumentation"));
                builder.AddOtlpExporter(otlpOptions =>
                {
                    // Use IConfiguration directly for Otlp exporter endpoint option.
                    //otlpOptions.Endpoint = new Uri(appBuilder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!);
                });
    })
    .WithMetrics(builder =>
    {
        // Metrics

        // Ensure the MeterProvider subscribes to any custom Meters.
        builder
//            .AddMeter(Instrumentation.MeterName)
            .SetExemplarFilter(ExemplarFilterType.TraceBased)
            .AddRuntimeInstrumentation()
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation();

                builder.AddOtlpExporter(otlpOptions =>
                {
                    // Use IConfiguration directly for Otlp exporter endpoint option.
                   // otlpOptions.Endpoint = new Uri(appBuilder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!);
                });
    })
    .WithLogging(builder =>
    {
        // Note: See appsettings.json Logging:OpenTelemetry section for configuration.

                builder.AddOtlpExporter(otlpOptions =>
                {
                    // Use IConfiguration directly for Otlp exporter endpoint option.
                    //otlpOptions.Endpoint = new Uri(appBuilder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!);
                });

    });


appBuilder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
appBuilder.Services.AddEndpointsApiExplorer();
appBuilder.Services.AddSwaggerGen();

var app = appBuilder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

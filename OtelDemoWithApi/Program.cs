using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient("for-external");
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("MyApp"))
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation();
        tracing.AddHttpClientInstrumentation();
        tracing.AddOtlpExporter(options =>
        {
            
        });
    })
    .WithMetrics(metrics =>
    {

        metrics.AddRuntimeInstrumentation();
        metrics.AddAspNetCoreInstrumentation();
        metrics.AddHttpClientInstrumentation();
        metrics.AddOtlpExporter((configure, metricReaderOptions) =>
        {
            metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 1000;
        });
    })
    .WithLogging(logging =>
    {
        logging.AddOtlpExporter();
    });


var app = builder.Build();

// Configure the HTTP request pipeline.

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (ILoggerFactory loggerFactory,IHttpClientFactory httpClientFactory) =>
{

    var logger = loggerFactory.CreateLogger("weathearforecast");

    logger.LogInformation("weatherforecast endpoint called");
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    using var httpClient = httpClientFactory.CreateClient("for-external");
    try
    {
        logger.LogInformation("Invoking external API");
        httpClient.GetAsync("https://www.google.com").GetAwaiter().GetResult();
    }
    catch (Exception exp)
    {
        logger.LogError(exp, "External API invocation failed");

    }
    return forecast;
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(9091);
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddEventLog();
// Add services to the container.
builder.Host.UseWindowsService(options =>
{
    options.ServiceName = "WinSvcApiServer";

});
var app = builder.Build();

// Configure the HTTP request pipeline.

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.MapGet("/write/{filename}", (string filename, ILogger<Program> logger) =>
{
    string status = string.Empty;
    logger.LogInformation("Received Request to write to {file}", filename);
    try
    {
        File.AppendAllText(filename, $"Hello {DateTime.Now.ToString()}\n");
        status = "Write Success";
    }
    catch (Exception exp)
    {

        logger.LogError(exp, "Write failed");
        status = $"Write failed {exp}";
    }

    return status;
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

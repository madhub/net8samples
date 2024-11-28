using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Connections;
using System.Net.Sockets;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(kestrelServerOptions =>
{
    // TCP 2025
    kestrelServerOptions.ListenAnyIP(2025, listenOption =>
    {
        // https://learn.microsoft.com/en-us/dotnet/core/additional-tools/self-signed-certificates-guide#with-dotnet-dev-certs
        listenOption.UseHttps (@"C:\Users\bs_ma\.aspnet\https\demoaspnetapp.pfx", "password")
        .UseConnectionLogging().UseConnectionHandler<MllpConnectionHandler>();
   
    });
});


 // Add services to the container.

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

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

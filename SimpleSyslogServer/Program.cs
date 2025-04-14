using Serilog;
using SimpleSyslogServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/syslog-.log",
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: 10_000_000, // 10 MB
        rollOnFileSizeLimit: true,
        retainedFileCountLimit: 10,     // Keep last 10 files
        shared: true)
    .CreateLogger();

// Replace default logging with Serilog
builder.Host.UseSerilog();
builder.Services.AddHostedService<SyslogUdpListener>();
var app = builder.Build();


// Configure the HTTP request pipeline.

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//});
app.MapGet("/", () => "Syslog UDP Server is running.");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

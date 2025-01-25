using System.Reflection;
using System.Runtime.InteropServices;
var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
var logFile = Path.Combine(baseDir, "ouput.log");

File.AppendAllText(logFile, $"Framework Version {RuntimeInformation.FrameworkDescription}\n");
Console.WriteLine();

var dict = Environment.GetEnvironmentVariables();

var envs = string.Join(Environment.NewLine, dict.Keys.Cast<string>().Select(k => k + " = " + dict[k]));
File.AppendAllText(logFile, envs);



var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddHealthChecks();
builder.Host.UseWindowsService(configure =>
{
    configure.ServiceName = "simplesrv";
});
var app = builder.Build();

// Configure the HTTP request pipeline.

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapHealthChecks("/health");
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
app.MapGet("/currenttime", async () =>
{
    await Task.Delay(500);
    return DateTime.Now.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss.fffffffK");
});
//AssemblyLoader.PrintAssemblyLoadPaths();
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

class AssemblyLoader
{
    public static void PrintAssemblyLoadPaths()
    {
        foreach (Assembly loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            string location = loadedAssembly.Location;
            Console.WriteLine($"Assembly: {loadedAssembly.FullName}");
            Console.WriteLine($"Load Path: {location}");
            Console.WriteLine("--------------------");
        }
    }
}
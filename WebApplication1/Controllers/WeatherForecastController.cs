using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.ExceptionSummarization;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IExceptionSummarizer exceptionSummarizer;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,IHttpClientFactory httpClientFactory, IExceptionSummarizer exceptionSummarizer)
    {
        _logger = logger;
        this.httpClientFactory = httpClientFactory;
        this.exceptionSummarizer = exceptionSummarizer;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        using var httpClient = httpClientFactory.CreateClient("demo");
        try
        {
            _logger.LogInformation("Invoking external api call ");
            httpClient.GetStringAsync("www.google.com1").GetAwaiter().GetResult();
        }
        catch (Exception exp)
        {
            ExceptionSummary summary = exceptionSummarizer.Summarize(exp);
            Console.WriteLine($"external api call failed {summary}");

        }
        
        
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}

public class OpenTelemetryLoggerOptions
{

}
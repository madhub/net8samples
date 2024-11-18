using Microsoft.AspNetCore.Mvc;
using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using System.Diagnostics.Tracing;

namespace DemoApp1.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    //[HttpGet(Name = "GetWeatherForecast")]
    //public IEnumerable<WeatherForecast> Get()
    //{
    //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    //    {
    //        Date = DateTime.Now.AddDays(index),
    //        TemperatureC = Random.Shared.Next(-20, 55),
    //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    //    })
    //    .ToArray();
    //}
    [HttpGet(Name = "DiagInfo")]
    public ActionResult<String> DiagInfo()
    {
        var providers = new List<EventPipeProvider>()
        {
            new EventPipeProvider(
                "System.Runtime",
                EventLevel.Informational,
                (long)ClrTraceEventParser.Keywords.None,
                new Dictionary<string, string>
                {
                    ["EventCounterIntervalSec"] = "1"
                }
            )
        };
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;
        DiagnosticsClient client = new DiagnosticsClient(Environment.ProcessId);
        using (EventPipeSession session = client.StartEventPipeSession(providers, false))
        {
            var source = new EventPipeEventSource(session.EventStream);

            source.Clr.All += (TraceEvent obj) =>
            {
                if (obj.EventName.Equals("EventCounters"))
                {
                    var payloadVal = (IDictionary<string, object>)(obj.PayloadValue(0));
                    var payloadFields = (IDictionary<string, object>)(payloadVal["Payload"]);
                    var name = string.Intern(payloadFields["Name"].ToString());

                    if (name.Equals("cpu-usage"))
                    {
                        double cpuUsage = Double.Parse(payloadFields["Mean"].ToString());
                        Console.WriteLine($"Cpu Usage {cpuUsage}");
                        tokenSource.Cancel();
                    }
                }
            };

            try
            {
                source.Process();
                Console.WriteLine("Waiting for cpu usage event...");
                token.WaitHandle.WaitOne();
                Console.WriteLine("Done processing the events");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error encountered while processing events");
                Console.WriteLine(e.ToString());
            }
        }

        return Ok("");
    }
}

sealed class EventSourceCreatedListener : EventListener
{
    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        base.OnEventSourceCreated(eventSource);
        Console.WriteLine($"New event source: {eventSource.Name}");
        if (eventSource.Name == "System.Runtime counters        ")
        {
            EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.All);
        }
    }
    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        base.OnEventWritten(eventData);
        var payloadVal = (IDictionary<string, object>)(eventData.Payload(0));
        var payloadFields = (IDictionary<string, object>)(payloadVal["Payload"]);
        var name = string.Intern(payloadFields["Name"].ToString());

        if (name.Equals("cpu-usage"))
        {
            double cpuUsage = Double.Parse(payloadFields["Mean"].ToString());
            Console.WriteLine($"Cpu Usage {cpuUsage}");
            tokenSource.Cancel();
        }
    }
}

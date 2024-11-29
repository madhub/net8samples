using Microsoft.AspNetCore.Connections;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(2020, listenOptions =>
    {
        listenOptions.UseConnectionLogging();
        listenOptions.UseHttps("certificate.pfx", "PaSswoRd~@34");
        listenOptions.Run(async ctx =>
        {
            Console.WriteLine($"[{ctx.RemoteEndPoint}]: connected");
            var reader = ctx.Transport.Input;
            try
            {
                while (true)
                {
                    ReadResult result = await reader.ReadAsync();
                    ReadOnlySequence<byte> buffer = result.Buffer;

                    while (TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
                    {
                        // Process the line.
                        ProcessLine(line);
                    }

                    // Tell the PipeReader how much of the buffer has been consumed.
                    reader.AdvanceTo(buffer.Start, buffer.End);

                    // Stop reading if there's no more data coming.
                    if (result.IsCompleted)
                    {
                        break;
                    }
                }
                // Mark the PipeReader as complete.
                await reader.CompleteAsync();
            }
            catch (Exception exp)
            {
                Console.WriteLine($"[{ctx.RemoteEndPoint}]: Exception while processing {exp.Message}");
            }
            Console.WriteLine($"[{ctx.RemoteEndPoint}]: disconnected");

            static bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
            {
                // Look for a EOL in the buffer.
                SequencePosition? position = buffer.PositionOf((byte)'\n');

                if (position == null)
                {
                    line = default;
                    return false;
                }

                // Skip the line + the \n.
                line = buffer.Slice(0, position.Value);
                buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
                return true;
            }
            static void ProcessLine(in ReadOnlySequence<byte> buffer)
            {
                foreach (var segment in buffer)
                {
                    Console.Write(Encoding.UTF8.GetString(buffer));
                }
                Console.WriteLine();
            }

        });
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

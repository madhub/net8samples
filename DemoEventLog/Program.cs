// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Logging;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.ClearProviders();
    builder.AddEventSourceLogger();
});
var logger = loggerFactory.CreateLogger("Demo Log Event");
logger.LogInformation("Hello From Demo");




using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

Console.WriteLine("Hello, Microsoft.Extensions.AI with Ollama & Tool calling!");
var message = "Search for patients scanned in last 7 day's. Use Current Date time";
Console.WriteLine(message);

using var factory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));

// 👇🏼 Use Ollama as the chat client
var ollamaChatClient = new OllamaChatClient(new Uri("http://localhost:11434/"), "llama3.2:3b");
var client = new ChatClientBuilder(ollamaChatClient)
    // 👇🏼 Add logging to the chat client, wrapping the function invocation client 
    .UseLogging(factory)
    // 👇🏼 Add function invocation to the chat client, wrapping the Ollama client
    .UseFunctionInvocation()
    .Build();

var chatOptions = new ChatOptions
{
    // 👇🏼 Define tools that can be used by the chat client
    Tools =
    [
        AIFunctionFactory.Create(GetCurrentTime),
        AIFunctionFactory.Create(GetCurrentWeather),
        AIFunctionFactory.Create(GetCurrentDateTime),
        AIFunctionFactory.Create(FindPatients)
    ]
};

var response = await client.GetResponseAsync(message, chatOptions);

Console.Write(response);

// 👇🏼 Define a time tool
[Description("Get the current time for a city")]
string GetCurrentTime(string city)
{
    return $"It is {DateTime.Now.Hour}:{DateTime.Now.Minute} in {city}.";
}

// 👇🏼 Define a weather tool
[Description("Get the current weather for a city")]
string GetCurrentWeather(string city)
{
    return $"The weather in {city} is sunny.";
}

// 👇🏼 Define a time tool
[Description("Get Current DateTime")]
String GetCurrentDateTime()
{
    return DateTime.Now.ToString();
}

// 👇🏼 Define a weather tool
[Description("Search for patients based on patient name,patient id , between given study dates . Use current date time for dates")]
string[] FindPatients(String ?patientName,String ?patientId,DateTime ?from ,DateTime ?to)
{
    if (string.IsNullOrEmpty(patientName)) {
        Console.WriteLine($"Searching based on patient name {patientName}");
    }
    if (string.IsNullOrEmpty(patientId))
    {
        Console.WriteLine($"Searching based on patient name {patientId}");
    }
    


    return new [] {"Joey","Chandler" };
}
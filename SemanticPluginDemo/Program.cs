// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.MistralAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticPluginDemo;
using System.Net;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    // Add OpenTelemetry as a logging provider
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Warning);
});

#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var builder = Kernel.CreateBuilder()
                    .AddMistralChatCompletion("mistral","apikey", new Uri("http://127.0.0.1:11434"));
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Plugins.AddFromType<LightPlugin>();
builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace));
Kernel kernel = builder.Build();


// Create chat history
var history = new ChatHistory();

// Get chat completion service
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Start the conversation
Console.Write("User > ");
string? userInput;
while ((userInput = Console.ReadLine()) != null)
{
    // Add user input
    history.AddUserMessage(userInput);

    // Enable auto function calling
#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    MistralAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        ToolCallBehavior = MistralAIToolCallBehavior.AutoInvokeKernelFunctions
    };
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


    // Get the response from the AI
    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    // Print the results
    Console.WriteLine("Assistant > " + result);

    // Add the message from the agent to the chat history
    history.AddMessage(result.Role, result.Content ?? string.Empty);

    // Get user input again
    Console.Write("User > ");
}
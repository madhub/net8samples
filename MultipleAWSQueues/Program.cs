using Microsoft.Extensions.DependencyInjection;
using MultipleAWSQueues;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAWSMessageBus(builder =>
{
    builder.AddSQSPoller("https://sqs.us-east-1.amazonaws.com/012345678910/MyQueue1");
    builder.AddMessageHandler<ChatMessageHandler, ChatMessage>("chatMessage");

    builder.AddSQSPoller("https://sqs.us-east-1.amazonaws.com/012345678910/SomeOtherQueue");
    builder.AddMessageHandler<ChatMessageHandler, ChatMessage>("chatMessage");
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();


using Microsoft.Extensions.Diagnostics.ExceptionSummarization;
using WebApplication1;

////var listener = new LoggingEventListener();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddExceptionSummarizer(b => b.AddHttpProvider());
builder.Services.AddHttpClient("demo");
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

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RestApiDemo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHostedService<SmsNotifierBackgroundService>();
builder.Services.AddHostedService<EmailNotifierBackgroundService>();
builder.Services.AddHostedService<PushNotifierBackgroundService>();
builder.Services.AddKeyedSingleton<INotificationService, SmsNotificationService>("sms");
builder.Services.AddKeyedSingleton<INotificationService, EmailNotificationService>("email");
builder.Services.AddKeyedSingleton<INotificationService, PushNotificationService>("push");

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResponseWriter = WriteMinimalPlaintext
 
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

static Task WriteMinimalPlaintext(HttpContext httpContext, HealthReport result)
{
    httpContext.Response.ContentType = "text/plain";
    return httpContext.Response.WriteAsync("Success - DateTime " + DateTime.Now.ToString());

}

using EfJobProcessingDemo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
// https://youtu.be/wjLJClV0awc
// dotnet tool install --global dotnet-ef
//dotnet add package Microsoft.EntityFrameworkCore.Design
//dotnet ef migrations add InitialCreate
//dotnet ef database update

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EfContext>(options =>
{
    options.UseSqlite("Data Source=mydemo.db");
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.MapGet("/demo", (EfContext efContext) =>
{
        
    return Results.Ok(efContext.Jobs);
});
app.MapPost("/demo", ([FromBody] ImportJob model, EfContext efContext) =>
{
    ImportJob importJob = new ImportJob()
    {
         DomainId = Guid.NewGuid(),
         JobParams = model.JobParams,
         CreatedAt = DateTime.Now,
         Status = JobStatus.Enqueued
    };
    efContext.Jobs.Add(importJob);
    efContext.SaveChanges();


    return Results.Ok(efContext.Jobs);
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

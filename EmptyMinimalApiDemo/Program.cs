// https://learning.oreilly.com/library/view/architecting-asp-net-core/9781805123385/text/ch006.xhtml#:-:text=Minimal%20Hosting
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");
// implicitly bind the id parameter from the route (highlighted code) to a parameter in the delegate:
app.MapGet(

    "minimal-endpoint-input-route-implicit/{id}",

    (int id) => $"The id was {id}."

);
// explicit bind the id parameter from the route (highlighted code) to a parameter in the delegate:
app.MapGet(

    "minimal-endpoint-input-route-explicit/{id}",

    ([FromRoute] int id) => $"The id was {id}."

);
app.MapGet(

    "minimal-endpoint-input-HttpContext/",

    (HttpContext context, CancellationToken cancellationToken, ClaimsPrincipal claimsPrincipals) =>
    {
        context.Response.WriteAsync("HttpContext!");
    }
    

);

app.MapGet(

    "minimal-endpoint-input-HttpResponse/",

    (HttpResponse response)

        => response.WriteAsync("HttpResponse!")

);

app.Run();

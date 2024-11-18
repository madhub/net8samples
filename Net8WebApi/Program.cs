var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();
// write a function to get HOSTNAME from environment variable 

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet(

    "minimal-endpoint-output-coordinate-ok1/",

    () => Results.Ok(new Coordinate {

        Latitude = 43.653225,

        Longitude = -79.383186

    })

);

app.MapGet(

    "minimal-endpoint-output-coordinate-ok2/",

    () => TypedResults.Ok(new Coordinate {

        Latitude = 43.653225,

        Longitude = -79.383186

    })

);
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

public record Coordinate 
{
    
    public required double Latitude { get; init; }
    public required double Longitude { get; init; }
}
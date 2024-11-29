// See https://aka.ms/new-console-template for more information


using KestralTcpServer;

MyDemoTcpServer myDemoTcpServer = new MyDemoTcpServer(2020);
await myDemoTcpServer.StartServer(args);
//var builder = WebApplication.CreateSlimBuilder(args); // Calls UseKestrelCore
////builder.WebHost.UseKestrelHttpsConfiguration();
//builder.Configuration.Sources.Clear();
//builder.Configuration.SetBasePath(AppContext.BaseDirectory);
//builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
//var app = builder.Build();
//app.MapGet("/{name}", (string name) => $"Hello {name}!");
//app.MapGet("/", () => $"Hello World");
//await app.RunAsync();

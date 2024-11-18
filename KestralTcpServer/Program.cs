// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
//builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
host.Run();

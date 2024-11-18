using DemoApi;
using k8s;




    using IronQr;
    using IronSoftware.Drawing;
     
    // Open the asset to read a QR Code from
    var inputBmp = AnyBitmap.FromFile(@"C:\madhu-docs\house-construction\068548ce4ea02a3a8cf3bba0f10cb0f5.png");
     
    // Load the asset into QrImageInput
    QrImageInput imageInput = new QrImageInput(inputBmp);
     
    // Create a QR Reader object
    QrReader reader = new QrReader();
     
    // Read the Input an get all embedded QR Codes
    IEnumerable<QrResult> results = reader.Read(imageInput);
    foreach(QrResult result in results) {
        Console.WriteLine(result.ToString());
    }
// var builder = WebApplication.CreateBuilder(args);
// builder.Logging.ClearProviders();
// builder.Logging.AddConsole();
// // Add services to the container.

// builder.Services.AddControllers();
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// // Load kubernetes configuration
// var kubernetesClientConfig = KubernetesClientConfiguration.BuildDefaultConfig();
// // Register Kubernetes client interface as sigleton
// builder.Services.AddSingleton<IKubernetes>(_ => new Kubernetes(kubernetesClientConfig));

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
// builder.Services.AddSingleton<AppInfo>();
// builder.Services.AddHttpClient("for_config_refresh");

// var app = builder.Build();
// var appInfo = app.Services.GetRequiredService<AppInfo>();
// app.Logger.LogInformation("Application Detail : Name:{name},Ip: {ip}",appInfo.AppName, appInfo.IPAddress);

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseAuthorization();

// app.MapControllers();

// app.Run();

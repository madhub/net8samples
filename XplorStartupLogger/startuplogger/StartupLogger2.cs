namespace XplorStartupLogger.startuplogger;

public class StartupLogger2
{
    private static ILoggerFactory _startupLoggerFactory;
    public StartupLogger2()
    {
        var cfg = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", optional: false)
                .Build();
        _startupLoggerFactory = LoggerFactory.Create(loggingBuilder =>
        {
            loggingBuilder.AddConfiguration(cfg);
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConsole();
            //loggingBuilder.Services.AddSingleton<ILoggerProvider>(memoryLoggerProvider);
        });
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            Console.WriteLine("AppDomain.CurrentDomain.ProcessExit Called");
            DisposeStartupLogger(_startupLoggerFactory);
        };
        /// register for Cancel.SIGINT/SIGTERM
        Console.CancelKeyPress += (_, ea) =>
        {
            Console.WriteLine("Console.CancelKeyPress Received");
            DisposeStartupLogger(_startupLoggerFactory);
        };

        static void DisposeStartupLogger(ILoggerFactory loggerFactory)
        {
            if (loggerFactory != null)
            {
                // any pending logs should be drained to console
                loggerFactory.Dispose();
                loggerFactory = null;
            }
        }
    }

    
}


using XplorStartupLogger.startuplogger;

namespace XplorStartupLogger;

public class ApplicationInitializer : BackgroundService
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<ApplicationInitializer> logger;
    private readonly ILogger bootStrapLogger;

    public ApplicationInitializer(ILoggerFactory loggerFactory,ILogger<ApplicationInitializer> logger)
    {
        this.loggerFactory = loggerFactory;
        this.logger = logger;
        bootStrapLogger = loggerFactory.CreateLogger("BootstrapLogger");
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
     
        throw new NotImplementedException();
    }
}

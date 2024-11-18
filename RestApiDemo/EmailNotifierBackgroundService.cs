
namespace RestApiDemo;

public class EmailNotifierBackgroundService : BackgroundService
{
    private readonly INotificationService emailNotifier;
    private readonly TimeSpan _period = TimeSpan.FromSeconds(200);
    public EmailNotifierBackgroundService([FromKeyedServices("email")] INotificationService emailNotifier)
    {
        this.emailNotifier = emailNotifier;
    }
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(_period);
        while (
            !stoppingToken.IsCancellationRequested &&
            await timer.WaitForNextTickAsync(stoppingToken))
        {
            emailNotifier.Notify($"[{nameof(EmailNotifierBackgroundService)}]{DateTime.Now.ToString()}");
        }
    }
}

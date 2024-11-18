
namespace RestApiDemo;

public class SmsNotifierBackgroundService : BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromSeconds(200);
    private readonly INotificationService smsNotifier;

    public SmsNotifierBackgroundService([FromKeyedServices("sms")] INotificationService smsNotifier)
    {
        this.smsNotifier = smsNotifier;
    }
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(_period);
        while (
            !stoppingToken.IsCancellationRequested &&
            await timer.WaitForNextTickAsync(stoppingToken))
        {
            smsNotifier.Notify($"[{nameof(SmsNotifierBackgroundService)}]{DateTime.Now.ToString()}");
        }
    }
}
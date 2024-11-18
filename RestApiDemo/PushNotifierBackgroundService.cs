
namespace RestApiDemo;

public class PushNotifierBackgroundService : BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromSeconds(200);
    private readonly INotificationService pushNotifier;

    public PushNotifierBackgroundService([FromKeyedServices("push")] INotificationService pushNotifier)
    {
        this.pushNotifier = pushNotifier;
    }
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(_period);
        while (
            !stoppingToken.IsCancellationRequested &&
            await timer.WaitForNextTickAsync(stoppingToken))
        {
            pushNotifier.Notify($"[{nameof(PushNotifierBackgroundService)}]{DateTime.Now.ToString()}");
        }
    }
}

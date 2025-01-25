namespace PeriodicTimerWithDynamicConfigUpdate;

using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
public class BackgroundServiceSettings
{
    public int IntervalInSeconds { get; set; }
}
public class MyBackgroundService : BackgroundService
{
    private readonly IOptionsMonitor<BackgroundServiceSettings> _optionsMonitor;
    private PeriodicTimer _timer;
    private int _intervalInSeconds;

    public MyBackgroundService(IOptionsMonitor<BackgroundServiceSettings> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
        _intervalInSeconds = _optionsMonitor.CurrentValue.IntervalInSeconds;
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(_intervalInSeconds));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Watch for changes in the configuration
        _optionsMonitor.OnChange(settings =>
        {
            _intervalInSeconds = settings.IntervalInSeconds;
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(_intervalInSeconds));
            Console.WriteLine($"Interval updated to {_intervalInSeconds} seconds.");
        });

        // Main loop for background service
        while (!stoppingToken.IsCancellationRequested)
        {
            if (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                Console.WriteLine($"Message printed every {_intervalInSeconds} seconds.");
            }
        }
    }
}

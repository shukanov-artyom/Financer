using MassTransit;

namespace ServiceOne;

public class Worker : BackgroundService
{
    private readonly IBus _bus;

    public Worker(IBus bus)
    {
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _bus.Publish(new Message { Text = $"The time is {DateTimeOffset.Now}" });
            await Task.Delay(550, stoppingToken);
        }
    }
}

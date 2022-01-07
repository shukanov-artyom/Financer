using MassTransit;

namespace ServiceOne;

public class Message
{
    public string Text { get; set; } = "";
}

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
            var now = DateTimeOffset.Now;
            await _bus.Publish(new Message { Text = $"The time is {now}" });
            Console.WriteLine($"Sending message {now}");
            await Task.Delay(550, stoppingToken);
        }
    }
}

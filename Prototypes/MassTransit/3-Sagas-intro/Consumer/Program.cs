using ServiceOne;
using MassTransit;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
        {
            // docker run -p 15672:15672 -p 5672:5672 masstransit/rabbitmq
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });
        services.AddMassTransitHostedService(true);
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();

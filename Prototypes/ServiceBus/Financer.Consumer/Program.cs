using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;

namespace Financer.Consumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAzureClients(b =>
            {
                b.AddServiceBusClient(builder.Configuration.GetConnectionString("ServiceBus"));
            });

            builder.Services.AddHostedService<ConsumerWorker>();

            var app = builder.Build();

            app.Run();
        }
    }
}
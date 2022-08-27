using Financer.Common.Messaging;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Financer.Common
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddFinancerCommons(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            serviceCollection.AddAzureClients(b =>
            {
                b.AddServiceBusClient(configuration.GetConnectionString("ServiceBus"));
            });
            serviceCollection.AddTransient<IServiceBusSender, ServiceBusSender>();

            return serviceCollection;
        }
    }
}

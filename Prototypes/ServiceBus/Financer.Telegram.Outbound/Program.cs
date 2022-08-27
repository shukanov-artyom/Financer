using Microsoft.Extensions.Azure;
using Telegram.Bot;

namespace Financer.Telegram.Outbound
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();
            builder.Services.AddHostedService<TelegramOutboundWorker>();
            builder.Services.AddAzureClients(b =>
            {
                b.AddServiceBusClient(builder.Configuration.GetConnectionString("ServiceBus"));
            });
            builder.Services.AddTransient<ITelegramBotClient>(c =>
                new TelegramBotClient(builder.Configuration["BotToken"]));

            var app = builder.Build();

            app.Run();
        }
    }
}
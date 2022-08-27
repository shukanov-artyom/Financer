using Financer.Common;
using Financer.Inbound.Services;
using Financer.Telegram.Inbound.Services.MessageProcessors;
using Microsoft.Extensions.Azure;
using Telegram.Bot;

namespace Financer.Inbound
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSingleton<ITelegramMessageProcessor, TextMessageProcessor>();
            builder.Services.AddSingleton<ITelegramMessageProcessor, VoiceMessageProcessor>();

            builder.Services.AddSingleton<IVoiceFilePathRetriever, TelegramOggFilePathRetriever>();
            builder.Services.AddTransient<ITelegramBotClient>(c => 
                new TelegramBotClient(builder.Configuration["BotToken"]));

            builder.Services.AddFinancerCommons(builder.Configuration);

            builder.Services.AddAzureClients(b =>
            {
                b.AddBlobServiceClient(builder.Configuration.GetConnectionString("BlobStorage"));
            });

            var botToken = builder.Configuration["BotToken"];
            builder.Services.AddHttpClient("TelegramBotApi", httpClient =>
            {
                httpClient.BaseAddress = new Uri($"https://api.telegram.org/bot{botToken}/");
            });
            builder.Services.AddHttpClient("TelegramFileApi", httpClient =>
            {
                httpClient.BaseAddress = new Uri($"https://api.telegram.org/file/bot{botToken}/");
            });

            builder.Services.AddHostedService<TelegramInboundWorker>();

            var app = builder.Build();

            app.Run();
        }
    }
}
using Financer.Common;
using Financer.SpeechRecognition.Services;
using Microsoft.Extensions.Azure;

namespace Financer.SpeechRecognition
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddTransient<SubscriptionKeyHandler>();

            builder.Services.AddAzureClients(b =>
            {
                b.AddServiceBusClient(builder.Configuration.GetConnectionString("ServiceBus"));
                b.AddBlobServiceClient(builder.Configuration.GetConnectionString("BlobStorage"));
            });

            var clientBuilder = builder.Services.AddHttpClient("TranscriptionsApi", httpClient =>
            {
                var basePath = "https://northeurope.api.cognitive.microsoft.com/speechtotext/v3.0/transcriptions";
                httpClient.BaseAddress = new Uri(basePath);
            });
            clientBuilder.AddHttpMessageHandler<SubscriptionKeyHandler>();

            builder.Services.AddHostedService<SpeechRecognitionEnqueueWorker>();
            builder.Services.AddHostedService<SpeechRecognitionDequeueWorker>();
            builder.Services.AddTransient<IBatchTranscriptionService, BatchTranscriptionService>();
            builder.Services.AddFinancerCommons(builder.Configuration);

            var app = builder.Build();

            app.Run();
        }
    }
}
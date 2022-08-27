using Financer.Telegram.Inbound.Services.MessageProcessors;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Financer.Inbound
{
    public class TelegramInboundWorker : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IEnumerable<ITelegramMessageProcessor> _messageProcessors;

        public TelegramInboundWorker(
            IEnumerable<ITelegramMessageProcessor> messageProcessors,
            ITelegramBotClient botClient)
        {
            _botClient = botClient;
            _messageProcessors = messageProcessors;

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // receive all update types
            };

            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                CancellationToken.None);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update.Message is Message message)
                {
                    await Task.WhenAll(_messageProcessors
                        .Where(mp => mp.MessageType == message.Type)
                        .Select(mp => mp.Process(message))
                        .ToArray());
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("was blocked by the user"))
                {
                    // nothing
                }
                else
                {
                    if (update.Message is Message message)
                    {
                        _botClient.SendTextMessageAsync(message.Chat.Id, $"Ошибка: {ex.Message}").Wait();
                    }
                }
            }
        }

        private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApiRequestException apiRequestException)
            {
                await botClient.SendTextMessageAsync(123, apiRequestException.ToString());
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}

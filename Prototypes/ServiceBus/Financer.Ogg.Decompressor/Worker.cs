namespace Financer.Ogg.Decompressor
{
    public class OggDecompressorWorker : BackgroundService
    {
        private readonly ILogger<OggDecompressorWorker> _logger;

        public OggDecompressorWorker(ILogger<OggDecompressorWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
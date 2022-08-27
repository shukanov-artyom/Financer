namespace Financer.Ogg.Decompressor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<OggDecompressorWorker>();
                })
                .Build();

            host.Run();
        }
    }
}
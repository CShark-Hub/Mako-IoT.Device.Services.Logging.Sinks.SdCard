using MakoIoT.Device.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace MakoIoT.Device.Services.Logging.Sinks.SdCard
{
    public static class IocExtensions
    {
        public static void AddSdCardSink(this IServiceCollection serviceCollection, SdCardSinkConfig config)
        {
            var consoleSink = new SdCardSink(config);
            serviceCollection.AddSingleton(typeof(ILogSink), consoleSink);
        }
    }
}

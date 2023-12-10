
using static nanoFramework.System.IO.FileSystem.SDCard;

namespace MakoIoT.Device.Services.Logging.Sinks.SdCard
{
    public sealed class SdCardSinkConfig
    {
        public SDCardSpiParameters sdCardSpi { get; set; }
        public string FileName { get; set; }
    }
}

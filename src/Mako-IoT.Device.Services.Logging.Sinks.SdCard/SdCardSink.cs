using MakoIoT.Device.Services.Interface;
using System;
using System.Threading;
using nanoFramework.System.IO.FileSystem;
using System.IO;
using System.Text;
using nanoFramework.System.IO;

namespace MakoIoT.Device.Services.Logging.Sinks.SdCard
{
    public sealed class SdCardSink : ILogSink
    {
        private const string DriverLetter = "D";
        private readonly string logstPath;
        private readonly object lockObject = new object();
        private readonly SDCard mycard;
        private bool cardReady;
        private Stream stream;

        public SdCardSink(SdCardSinkConfig config)
        {
            logstPath = $"{DriverLetter}:\\{config.FileName}";
            mycard = new SDCard(config.sdCardSpi);

            if (mycard.CardDetectEnabled)
            {
                StorageEventManager.RemovableDeviceInserted += StorageEventManager_RemovableDeviceInserted;
                StorageEventManager.RemovableDeviceRemoved += StorageEventManager_RemovableDeviceRemoved;
            }

            MountMyCard();
        }

        public void Dispose()
        {
            mycard?.Dispose();
            stream?.Dispose();
        }

        public void Log(string message)
        {
            if (!cardReady)
            {
                Console.WriteLine("SD card is not ready. Skipping log to SD card.");
                return;
            }

            AppendToFile(logstPath, message);
        }

        public void AppendToFile(string filePath, string text)
        {
            lock (lockObject)
            {
                if (stream == null)
                {
                    stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    stream.Seek(0, SeekOrigin.End);
                }

                stream.Write(Encoding.UTF8.GetBytes(text), 0, text.Length);
                // End of line in UTF8
                stream.WriteByte(0x0D);
                stream.WriteByte(0x0A);
            }
        }

        void UnMountIfMounted()
        {
            if (!mycard.IsMounted)
            {
                return;
            }

            mycard.Unmount();
        }

        void MountMyCard(bool shouldWaitForInitialization = true)
        {
            try
            {
                if (shouldWaitForInitialization)
                {
                    Thread.Sleep(5000);
                }

                mycard.Mount();
                cardReady = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Card failed to mount: {ex.Message}");
                cardReady = false;
            }
        }

        private void StorageEventManager_RemovableDeviceRemoved(object sender, RemovableDriveEventArgs e)
        {
            cardReady = false;
            UnMountIfMounted();
        }

        private void StorageEventManager_RemovableDeviceInserted(object sender, RemovableDriveEventArgs e)
        {
            UnMountIfMounted();
            MountMyCard(false);
        }
    }
}

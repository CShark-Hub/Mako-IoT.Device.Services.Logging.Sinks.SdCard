using System;
using System.IO;
using MakoIoT.Device.Services.Server.WebServer;
using Microsoft.Extensions.Logging;
using System.Text;
using MakoIoT.Device.Platform.Interface;
using MakoIoT.Device.Platform.Interface.Configuration;
using MakoIoT.Device.Platform.LocalConfig.Extensions;
using MakoIoT.Device.Services.Configuration;
using MakoIoT.Device.Services.FileStorage.Interface;
using MakoIoT.Device.Services.Interface;
using MakoIoT.Device.Services.WiFi.Configuration;

namespace MakoIoT.Device.Platform.LocalConfig.Controllers
{
    public class IndexController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfigurationService _configService;
        private readonly IStreamStorageService _storageService;

        public IndexController(ILogger logger, IConfigurationService configService, IStreamStorageService storageService) 
            : base(HtmlResources.GetString(HtmlResources.StringResources.index))
        {
            _logger = logger;
            _configService = configService;
            _storageService = storageService;
        }

        [Route("")]
        [Route("index.html")]
        [Method("GET")]
        public void Get(WebServerEventArgs e)
        {
            try
            {
                var wifiConfig = (WiFiConfig)_configService.GetConfigSection(WiFiConfig.SectionName, typeof(WiFiConfig));
                HtmlParams.AddOrUpdate("ssid", wifiConfig.Ssid);
                HtmlParams.AddOrUpdate("password", wifiConfig.Password);
            }
            catch (ConfigurationException)
            {
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error loading wifi configuration");
            }

            try
            {
                var platformConfig = (PlatformConfig)_configService.GetConfigSection(PlatformConfig.SectionName, typeof(PlatformConfig));
                HtmlParams.AddOrUpdate("platformUrl", platformConfig.PlatformServiceUrl);
                HtmlParams.AddOrUpdate("apiKey", platformConfig.PlatformServiceApiKey);
            }
            catch (ConfigurationException)
            {
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error loading platform configuration");
            }

            Render(e.Context.Response, false);
        }

        [Route("")]
        [Route("index.html")]
        [Method("POST")]
        public void Post(WebServerEventArgs e)
        {
            try
            {
                ParseForm(e.Context.Request, (fieldName, fileName, reader, boundary) =>
                {
                    if (fieldName == "httpsCertFile")
                        return SaveFile(reader, boundary, Constants.PlatformCertificateFile);
                    if (fieldName == "deviceKeyFile")
                        return SaveFile(reader, boundary, Constants.DeviceEncryptionKeyFile);
                    return reader.ReadLine();
                });

                _configService.UpdateConfigSection(WiFiConfig.SectionName, new WiFiConfig
                {
                    Ssid = (string)Form["ssid"],
                    Password = (string)Form["password"],
                });

                _configService.UpdateConfigSection(PlatformConfig.SectionName, new PlatformConfig
                {
                    PlatformServiceUrl = (string)Form["platformUrl"],
                    PlatformServiceApiKey = (string)Form["apiKey"],
                });

                HtmlParams.AddOrUpdate("messages", GetMessage("success", "Configuration updated"));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error updating configuration");
                HtmlParams.AddOrUpdate("messages", GetMessage("danger", "Error updating configuration"));
            }

            Render(e.Context.Response, true);
        }

        private string SaveFile(StreamReader reader, string boundary, string fileName)
        {
            using var writer = _storageService.WriteToFileStream(fileName);
            string line = reader.ReadLine();
            while (line != null && !line.StartsWith(boundary))
            {
                writer.WriteLine(line);
                line = reader.ReadLine();
            }

            return line;
        }


        private static string GetMessage(string type, string text)
        {
            var html = new StringBuilder(HtmlResources.GetString(HtmlResources.StringResources.message));
            return html.Replace("{class}", type).Replace("{text}", text).ToString();
        }
    }
}

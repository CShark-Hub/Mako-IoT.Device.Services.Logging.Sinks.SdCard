
using MakoIoT.Device.Services.Server.WebServer;
using MakoIoT.Device.Services.ConfigurationManager;
using System.Threading;

namespace MakoIoT.Device.Platform.LocalConfig.Controllers
{
    public class ExitController : ControllerBase
    {
        private readonly IConfigManager _configManager;

        public ExitController(IConfigManager configManager)
            : base(HtmlResources.GetString(HtmlResources.StringResources.exit))
        {
            _configManager = configManager;
        }

        [Route("exit.html")]
        [Method("GET")]
        public void Get(WebServerEventArgs e)
        {
            Render(e.Context.Response, false);

            new Thread(() =>
            {
                Thread.Sleep(5000);
                _configManager.StopConfigMode();
            }).Start();
        }
    }
}

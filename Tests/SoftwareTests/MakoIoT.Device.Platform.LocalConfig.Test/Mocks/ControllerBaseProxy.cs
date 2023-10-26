using System.Collections;
using System.IO;
using MakoIoT.Device.Platform.LocalConfig.Controllers;

namespace MakoIoT.Device.Platform.LocalConfig.Test.Mocks
{
    public class ControllerBaseProxy : ControllerBase
    {
        public ControllerBaseProxy(string html) : base(html)
        {
        }

        public Hashtable BaseForm => Form;

        public void BaseParseForm(string contentType, long contentLength, Stream requestStream,
            FileUploadDelegate fileUploadDelegate = null)
        {
            ParseForm(contentType, contentLength, requestStream, fileUploadDelegate);
        }
    }
}

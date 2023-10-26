 using nanoFramework.TestFramework;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using MakoIoT.Device.Platform.LocalConfig.Controllers;
using MakoIoT.Device.Platform.LocalConfig.Test.Mocks;
using MakoIoT.Device.Platform.LocalConfig.Test.TestData;

namespace MakoIoT.Device.Platform.LocalConfig.Test
{
    [TestClass]
    public class ControllerBaseTest
    {
        private const string HTML = "";

        [TestMethod]
        public void ParseForm_given_urlencoded_form_should_populate_values()
        {
            string contentType = "application/x-www-form-urlencoded";
            var form = @"ssid=ssid_value&password=password_value&platformUrl=https%3A%2F%2Fplatform.com&apiKey=apikey_value";
            
            var sut = new ControllerBaseProxy(HTML);

            using var requestStream = new MemoryStream(Encoding.UTF8.GetBytes(form));

            long contentLength = requestStream.Length;
            sut.BaseParseForm(contentType, contentLength, requestStream);

            Assert.AreEqual(4, sut.BaseForm.Keys.Count);
            Assert.AreEqual("ssid_value", (string)sut.BaseForm["ssid"]);
            Assert.AreEqual("password_value", (string)sut.BaseForm["password"]);
            Assert.AreEqual("https://platform.com", (string)sut.BaseForm["platformUrl"]);
            Assert.AreEqual("apikey_value", (string)sut.BaseForm["apiKey"]);
        }

        [TestMethod]
        public void ParseForm_given_multipart_form_should_populate_values()
        {
            string contentType = "multipart/form-data; boundary=---------------------------367844683732833203791531579764";
            var form = FormData.FormMultipart;
        
            var sut = new ControllerBaseProxy(HTML);
        
            using var requestStream = new MemoryStream(Encoding.UTF8.GetBytes(form));
        
            long contentLength = requestStream.Length;
            sut.BaseParseForm(contentType, contentLength, requestStream);
        
            Assert.AreEqual(4, sut.BaseForm.Keys.Count);
            Assert.AreEqual("ssid1", (string)sut.BaseForm["ssid"]);
            Assert.AreEqual("password2", (string)sut.BaseForm["password"]);
            Assert.AreEqual("https://platform.com", (string)sut.BaseForm["platformUrl"]);
            Assert.AreEqual("key3", (string)sut.BaseForm["apiKey"]);
        }

        [TestMethod]
        public void ParseForm_given_multipart_form_should_extract_file()
        {
            string contentType = "multipart/form-data; boundary=---------------------------367844683732833203791531579764";
            var form = FormData.FormMultipart;

            var sut = new ControllerBaseProxy(HTML);

            using var requestStream = new MemoryStream(Encoding.UTF8.GetBytes(form));

            long contentLength = requestStream.Length;

            string fileName = "";
            string contents = "";

            sut.BaseParseForm(contentType, contentLength, requestStream, (fieldName, name, reader, boundary) =>
            {
                Assert.AreEqual("-----------------------------367844683732833203791531579764", boundary);
                Assert.AreEqual("httpsCertFile", fieldName);
                Debug.WriteLine(boundary);
                fileName = name;
                string line = reader.ReadLine();
                while (line != null && !line.StartsWith(boundary))
                {
                    Debug.WriteLine(line);
                    contents += $"{line}\r\n";
                    line = reader.ReadLine();
                }

                contents = contents.TrimEnd('\r', '\n');

                return line;
            });

            Assert.AreEqual("myfilename.txt", fileName);
            Assert.AreEqual(@"My File Contents
Line 2", contents);
        }
    }
}
    
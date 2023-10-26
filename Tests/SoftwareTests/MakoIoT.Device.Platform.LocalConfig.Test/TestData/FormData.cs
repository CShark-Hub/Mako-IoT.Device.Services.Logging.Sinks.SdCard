namespace MakoIoT.Device.Platform.LocalConfig.Test.TestData
{
    public static class FormData
    {
        public static string FormMultipart = @"-----------------------------367844683732833203791531579764
Content-Disposition: form-data; name=""ssid""

ssid1
-----------------------------367844683732833203791531579764
Content-Disposition: form-data; name=""password""

password2
-----------------------------367844683732833203791531579764
Content-Disposition: form-data; name=""platformUrl""

https://platform.com
-----------------------------367844683732833203791531579764
Content-Disposition: form-data; name=""apiKey""

key3
-----------------------------367844683732833203791531579764
Content-Disposition: form-data; name=""httpsCertFile""; filename=""myfilename.txt""
Content-Type: application/octet-stream

My File Contents
Line 2
-----------------------------367844683732833203791531579764--
 ";
    }
}

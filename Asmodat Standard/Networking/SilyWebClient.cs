using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AsmodatStandard.Networking
{
    public static class SilyWebClientEx
    {
        public static bool CheckInternetAccess(string aURL = "http://clients3.google.com/generate_204", int timeout = 10000)
        {
            try
            {
                return SilyWebClientEx.GetRequest(aURL: aURL, timeout: timeout) != null;
            }
            catch
            {
                return false;
            }
        }

        public static string GetRequest(string aURL, int timeout)
        {
            using (var lWebClient = new SilyWebClient())
            {
                lWebClient.Timeout = timeout;
                return lWebClient.DownloadString(aURL);
            }
        }
    }

    public class SilyWebClient : System.Net.WebClient
    {
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest lWebRequest = base.GetWebRequest(uri);
            lWebRequest.Timeout = Timeout;
            ((HttpWebRequest)lWebRequest).ReadWriteTimeout = Timeout;
            return lWebRequest;
        }
    }

    
}

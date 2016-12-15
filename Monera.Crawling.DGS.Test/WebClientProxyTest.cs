using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monera.Crawling.DGS.Crawlers;

namespace Monera.Crawling.DGS.Test
{
    [TestClass]
    public class WebClientProxyTest
    {
        [TestMethod]
        public void DownloadUrlTest()
        {
            WebProxy proxyString = new WebProxy("http://fr.proxymesh.com:31280", true);
            //set network credentials may be optional
            NetworkCredential proxyCredential = new NetworkCredential("dgs", "Carlsberg123");
            proxyString.Credentials = proxyCredential;
            //WebRequest.DefaultWebProxy = proxyString;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.krak.dk/autov%C3%A6rksteder/s%C3%B8g.cs");
            request.Proxy = proxyString;
            request.Method = "GET";
            //manually set authorization header
            string authInfo = "dgs" + ":" + "Carlsberg123";
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            request.Headers["Proxy-Authorization"] = "Basic " + authInfo;

            try
            {
                using (var stream = request.GetResponse().GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        var response = sr.ReadToEnd();
                        Debug.WriteLine(response);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}

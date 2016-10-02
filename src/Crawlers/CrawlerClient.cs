using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Monera.Crawling.DGS.Helpers;

namespace Monera.Crawling.DGS.Crawlers
{
    public class CrawlerClient : WebClient
    {
        public CrawlerClient()
        {
            var useProxy = ConfigurationHelper.GetValue("UseProxy", false);
            if (!useProxy)
                return;

            var proxyUrl = ConfigurationHelper.GetValue("ProxyUrl", string.Empty);
            if (string.IsNullOrEmpty(proxyUrl))
                return;

            var proxy =
                new WebProxy
                {
                    Address = new Uri(proxyUrl),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = ConfigurationHelper.GetValue("ProxyDefaultCredentials", true)
                };

            if (!proxy.UseDefaultCredentials)
            {
                proxy.Credentials =
                    new NetworkCredential(
                        ConfigurationHelper.GetValue<string>("ProxyLogin"),
                        ConfigurationHelper.GetValue<string>("ProxyPassword"));
            }

            this.Proxy = proxy;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest w = base.GetWebRequest(address);
            w.Timeout = 30 * 1000;
            return w;
        }
    }
}

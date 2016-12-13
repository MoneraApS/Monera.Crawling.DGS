using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monera.Crawling.DGS.Crawlers;

namespace Monera.Crawling.DGS
{
    public static class CrawlerFactory
    {
        public static BaseCrawler Create(string name)
        {
            switch (name)
            {
                case "degulesider.dk":
                    return new DgsCrawler();
                case "krak.dk":
                    return new KrakCrawler();
            }

            throw new InvalidOperationException($"Crawler '{name}' is not supported");
        }
    }
}

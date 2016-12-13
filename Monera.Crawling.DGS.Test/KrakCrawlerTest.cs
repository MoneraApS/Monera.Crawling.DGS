using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monera.Crawling.DGS.Crawlers;

namespace Monera.Crawling.DGS.Test
{
    [TestClass]
    public class KrakCrawlerTest
    {
        [TestMethod]
        public void TestCrawlerGetUrls()
        {
            var crawler = new KrakCrawler();
            crawler.GetUrls("http://www.krak.dk/autov%C3%A6rksteder/s%C3%B8g.cs");
        }

        [TestMethod]
        public void TestCrawlerRun()
        {
            var crawler = new KrakCrawler();
            crawler.Run("http://www.krak.dk/autov%C3%A6rksteder/s%C3%B8g.cs");
        }
    }
}

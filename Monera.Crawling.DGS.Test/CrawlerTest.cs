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
    public class CrawlerTest
    {
        [TestMethod]
        public void TestCrawlerGetUrls()
        {
            var crawler = new DgsCrawler();
            crawler.GetUrls("http://www.degulesider.dk/t%C3%B8mrere/s%C3%B8g.cs");
        }

        [TestMethod]
        public void TestCrawlerRun()
        {
            var crawler = new DgsCrawler();
            crawler.Run("http://www.degulesider.dk/t%C3%A6pperens/p:25/s%C3%B8g.cs");
        }

        [TestMethod]
        public void TestParseUrl()
        {
            var url = "http://www.degulesider.dk/t%C3%B8mrere/s%C3%B8g.cs";
            var parts = url.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            Assert.IsTrue(parts.Length == 4);
        }
    }
}

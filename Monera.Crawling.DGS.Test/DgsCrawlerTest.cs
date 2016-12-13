using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monera.Crawling.DGS.Crawlers;
using Monera.Crawling.DGS.Domain.Data;
using Monera.Crawling.DGS.Helpers;
using OfficeOpenXml;

namespace Monera.Crawling.DGS.Test
{
    [TestClass]
    public class DgsCrawlerTest
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
            crawler.Run("http://www.degulesider.dk/t%C3%A6pperens/p:4/s%C3%B8g.cs");
        }

        [TestMethod]
        public void TestParseUrl()
        {
            var url = "http://www.degulesider.dk/t%C3%B8mrere/s%C3%B8g.cs";
            var parts = url.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            Assert.IsTrue(parts.Length == 4);
        }

        [TestMethod]
        public void ExportTest()
        {
            using (var context = new DgsContext())
            {
                var data = context.CrawlItems.AsNoTracking().ToList();
                var excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromCollection(data, true);
                using (var fs = new FileStream(Path.Combine(ConfigurationHelper.GetValue<string>("OutputPathFolder"), DateTime.Now.Ticks + ".xlsx"), FileMode.OpenOrCreate))
                {
                    excel.SaveAs(fs);
                }
            }
        }
    }
}

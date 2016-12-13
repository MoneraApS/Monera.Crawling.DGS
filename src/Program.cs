using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monera.Crawling.DGS.Crawlers;
using Monera.Crawling.DGS.Domain.Data;
using Monera.Crawling.DGS.Domain.Models;
using Monera.Crawling.DGS.Helpers;
using OfficeOpenXml;

namespace Monera.Crawling.DGS
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var sites = new Dictionary<string, List<string>>();
                var dirInfo = new DirectoryInfo(ConfigurationHelper.GetValue<string>("PathFile"));
                foreach (var fileInfo in dirInfo.GetFiles())
                {
                    var urls = new List<string>();
                    using (var fs = new FileStream(fileInfo.FullName, FileMode.Open))
                    {
                        using (var reader = new StreamReader(fs, Encoding.UTF8))
                        {
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                if (string.IsNullOrEmpty(line)) continue;

                                urls.Add(line.Trim());
                            }
                        }
                    }
                    sites.Add(fileInfo.Name, urls);
                }

                var outputs = new List<CrawlItem>();
                using (var db = new DgsContext())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    outputs = db.CrawlItems.AsNoTracking().ToList();
                }

                foreach (var site in sites)
                {
                    Crawl(site, outputs);
                }

                try
                {
                    ExportExcel(outputs);
                }
                catch (Exception eex)
                {
                    Console.WriteLine("Export problem");
                    Console.WriteLine(eex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("DONE!");
            Console.ReadLine();
        }

        private static void Crawl(KeyValuePair<string, List<string>> site, List<CrawlItem> current)
        {
            if (site.Value.Count == 0) return;

            var urls = site.Value;
            var groups = new Dictionary<int, List<string>>();
            var list = new List<string>();
            for (int i = 0; i < urls.Count; i++)
            {
                if (i > 0 && i%2 == 0)
                {
                    groups.Add(i, list);
                    list = new List<string>();
                }
                list.Add(urls[i]);
            }
            if (list.Any()) groups.Add(urls.Count, list);

            var outputs = new List<CrawlItem>();
            foreach (var group in groups)
            {
                var outputs1 = outputs;
                Parallel.ForEach(group.Value, (url) =>
                {
                    var results = CrawlerFactory.Create(site.Key).Execute(url);
                    foreach (var result in results)
                    {
                        if (!result.Items.Any()) continue;
                        outputs1.AddRange(result.Items);
                    }
                });
            }

            var now = DateTime.UtcNow;
            outputs.ForEach(item => item.CreatedDate = now);

            using (var db = new DgsContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;

                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("Start save {0}", DateTime.Now);

                var missingRecords = outputs.Where(x => current.All(z => z.CompanyName != x.CompanyName)).ToList();
                db.CrawlItems.AddRange(missingRecords);
                db.SaveChanges();

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("End save {0}", DateTime.Now);
                Console.ForegroundColor = ConsoleColor.White;

                current.AddRange(missingRecords);
            }
        }

        private static void ExportExcel(List<CrawlItem> data)
        {
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

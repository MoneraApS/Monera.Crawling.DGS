using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monera.Crawling.DGS.Crawlers;
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
                var urls = new List<string>();
                using (var fs = new FileStream(ConfigurationHelper.GetValue<string>("PathFile"), FileMode.Open))
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

                if (urls.Any())
                {
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
                        Parallel.ForEach(group.Value, (url) =>
                        {
                            var results = (new DgsCrawler()).Execute(url);
                            foreach (var result in results)
                            {
                                if (!result.Items.Any()) continue;
                                outputs.AddRange(result.Items);
                            }
                        });
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("DONE!");
            Console.ReadLine();
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monera.Crawling.DGS.Crawlers;
using Monera.Crawling.DGS.Helpers;

namespace Monera.Crawling.DGS
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var urls = new List<string>();
                using (var fs = new FileStream(ConfigurationHelper.GetValue<string>(""), FileMode.Open))
                {
                    using (var reader = new StreamReader(fs, Encoding.UTF8))
                    {
                        var line = reader.ReadLine();
                        while (!reader.EndOfStream)
                        {
                            line = reader.ReadLine();
                            if (string.IsNullOrEmpty(line)) continue;

                            urls.Add(line.Trim());
                        }
                    }
                }

                if (urls.Any())
                {
                    Parallel.ForEach(urls, (url) =>
                    {
                        (new DgsCrawler()).Execute(url);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine("DONE!");
            Console.ReadLine();
        }
    }
}

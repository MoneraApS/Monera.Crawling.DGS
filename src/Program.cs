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

                    foreach (var group in groups)
                    {
                        Parallel.ForEach(group.Value, (url) =>
                        {
                            (new DgsCrawler()).Execute(url);
                        });
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
    }
}

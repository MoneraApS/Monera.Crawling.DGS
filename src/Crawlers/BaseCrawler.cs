using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monera.Crawling.DGS.Domain.Data;

namespace Monera.Crawling.DGS.Crawlers
{
    public abstract class BaseCrawler
    {
        private const int csProcessThreshold = 1000; // Size of block to load data
        private static readonly int csCommandTimeout = 180;

        public List<CrawlerResult> Execute(string url)
        {
            try
            {
                var pageUrls = GetUrls(url);
                var tasks = pageUrls.Select(pageUrl => Task<CrawlerResult>.Factory.StartNew((link) => Run((string) link), pageUrl)).ToArray();
                Task.WaitAll(tasks);

                var results = tasks.Select(task => task.Result).ToList();
                if (!results.Any()) return new List<CrawlerResult>();

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return new List<CrawlerResult>();
        }

        public abstract List<string> GetUrls(string url);
        public abstract CrawlerResult Run(string url);
        public abstract string Name { get; }
    }
}

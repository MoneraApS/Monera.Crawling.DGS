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

                using (var db = new DgsContext())
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine("Start save {0}, {1}", url, DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;

                    AddItems(db, results);

                    db.BulkSaveChanges(bulk => bulk.BatchSize = 500);

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("End save {0}, {1}", url, DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                }

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
        protected abstract void AddItems(DgsContext context, List<CrawlerResult> crawlerResult);
    }
}

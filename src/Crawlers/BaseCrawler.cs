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

        public void Execute(string url)
        {
            try
            {
                var pageUrls = GetUrls(url);
                var tasks = pageUrls.Select(pageUrl => Task<CrawlerResult>.Factory.StartNew((link) => Run((string) link), pageUrl)).ToArray();
                Task.WaitAll(tasks);

                var results = tasks.Select(task => task.Result).ToList();
                if (!results.Any()) return;

                using (var db = new DgsContext())
                {
                    db.CommandTimeout = csCommandTimeout;
                    db.WithNOLOCK<int>((tx) =>  // select with unlock - much faster for bigdata
                    {
                        AddItems(db, results);

                        db.SaveChanges();
                        tx.Commit();
                        return 0;
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public abstract List<string> GetUrls(string url);
        public abstract CrawlerResult Run(string url);
        protected abstract void AddItems(DgsContext context, List<CrawlerResult> crawlerResult);
    }
}

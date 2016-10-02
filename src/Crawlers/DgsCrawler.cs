using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monera.Crawling.DGS.Domain.Data;

namespace Monera.Crawling.DGS.Crawlers
{
    public class DgsCrawler : BaseCrawler
    {
        protected override List<string> GetUrls(string url)
        {
            Console.WriteLine("Parse {0} to pagingUrls", url);
            return new List<string>();
        }

        protected override CrawlerResult Run(string url)
        {
            return new CrawlerResult();
        }

        protected override void AddItems(DgsContext context, List<CrawlerResult> crawlerResult)
        {
            foreach (var result in crawlerResult)
            {
                context.CrawlItems.AddRange(result.Items);
            }
        }
    }
}

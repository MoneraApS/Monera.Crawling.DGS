using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monera.Crawling.DGS.Domain.Models;

namespace Monera.Crawling.DGS.Crawlers
{
    public class CrawlerResult
    {
        public CrawlerResult()
        {
            Items = new List<CrawlItem>();
        }

        public List<CrawlItem> Items { get; private set; } 
    }
}

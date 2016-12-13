using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Monera.Crawling.DGS.Domain.Data;

namespace Monera.Crawling.DGS.Crawlers
{
    public class KrakCrawler : BaseCrawler
    {
        public override string Name => "krak.dk";

        public override List<string> GetUrls(string url)
        {
            Console.WriteLine("Parse {0} to pagingUrls", url);
            try
            {
                var html = string.Empty;

                try
                {
                    using (var client = new CrawlerClient())
                    {
                        client.Encoding = Encoding.UTF8;
                        var str = client.DownloadString(url);
                        html = str;
                    }
                }
                catch (Exception)
                {
                    return new List<string>();
                }

                if (string.IsNullOrEmpty(html))
                {
                    System.Console.WriteLine("Searching page at {0} don't have content", url);
                    return new List<string>();
                }

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var pageCount = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='main-content']/div[@class='paging']/ol[@class='page-container']/li[@class='page-count no-mobile']");
                if (pageCount == null) return new List<string> { url };

                var totalPages = int.Parse(pageCount.InnerHtml.Trim().Replace("... ", ""));
                var results = new List<string>();

                var parts = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                for (var i = 1; i <= totalPages; i++)
                {
                    results.Add($"{parts[0]}//{parts[1]}/{parts[2]}/p:{i}/{parts[3]}");
                }

                return results;
            }
            catch (Exception exp)
            {
                System.Console.WriteLine("Error in getting page urls info");
                System.Console.WriteLine(exp);
            }

            return new List<string>();
        }

        public override CrawlerResult Run(string url)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Monera.Crawling.DGS.Domain.Data;
using Monera.Crawling.DGS.Domain.Models;
using Polly;

namespace Monera.Crawling.DGS.Crawlers
{
    public class DgsCrawler : BaseCrawler
    {
        private string source;
        private string category;

        public override string Name => "degulesider.dk";

        public override List<string> GetUrls(string url)
        {
            this.source = url;
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

                var parts = url.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
                this.category = parts[2];
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
            Console.WriteLine("Parse {0}", url);
            var html = string.Empty;

            var policy = Policy.Handle<Exception>().WaitAndRetry(
                retryCount: 3, // Retry 3 times
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(3));

            try
            {
                policy.Execute(() =>
                {
                    using (var client = new CrawlerClient())
                    {
                        client.Encoding = Encoding.UTF8;
                        var str = client.DownloadString(url);
                        html = str;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Request error {0}", url);
                Console.WriteLine(ex);
                Console.ForegroundColor = ConsoleColor.White;
                return new CrawlerResult();
            }

            if (string.IsNullOrEmpty(html))
            {
                System.Console.WriteLine("Searching page at {0} don't have content", url);
                return new CrawlerResult();
            }

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var result = new CrawlerResult();
            var items = htmlDocument.DocumentNode.SelectNodes("//ol[@id='hit-list']/li[@class='hit-item']");
            if (items == null)
            {
                items = htmlDocument.DocumentNode.SelectNodes("//ol[@id='hit-list']/article"); // hit vcard default style-non-paying
            }
            if (items == null) return result;

            foreach (var item in items)
            {
                var crawlItem = new CrawlItem { SourceUrl = url, Source = this.source, Category = this.category, CompanyEmail = string.Empty };

                var companyName = item.SelectSingleNode(".//span[@class='hit-company-name-ellipsis']");
                if (!string.IsNullOrEmpty(companyName?.InnerText))
                {
                    crawlItem.CompanyName = companyName.InnerText.Trim(new char[] { ' ', '\n' });
                }

                var promoted =
                    item.SelectSingleNode(".//div[@class='hit-logotype-container']/*[contains(@class, 'hit-logotype-link')]");
                crawlItem.Promoted = promoted != null;

                var companyPhone =
                    item.SelectSingleNode(
                        ".//a[contains(@class, 'hit-phone-number')]");
                if (companyPhone?.Attributes["data-phone"] != null)
                {
                    crawlItem.CompanyPhone = companyPhone.Attributes["data-phone"].Value;
                    if (!string.IsNullOrEmpty(crawlItem.CompanyPhone))
                    {
                        crawlItem.CompanyPhone = crawlItem.CompanyPhone.Replace(" ", "");
                    }
                }
                    
                var companyAddress = item.SelectNodes(".//div[contains(@class, 'hit-company-location')]/descendant::span");
                if (companyAddress != null)
                {
                    crawlItem.CompanyAddress = string.Join(", ", companyAddress.Select(addressNode => addressNode.InnerText).Where(a => !string.IsNullOrEmpty(a)).Select(a => a).ToArray());
                }

                var marketingBlock = item.SelectSingleNode(".//a[@class='addax addax-cs_hl_hit_organization_a_click']");
                crawlItem.MarketingBlock = marketingBlock != null;

                result.Items.Add(crawlItem);
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Completed Parse {0}", url);
            Console.ForegroundColor = ConsoleColor.White;

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Monera.Crawling.DGS.Domain.Data;
using Monera.Crawling.DGS.Domain.Models;
using Newtonsoft.Json;
using Polly;

namespace Monera.Crawling.DGS.Crawlers
{
    public class KrakCrawler : BaseCrawler
    {
        public override string Name => "krak.dk";
        private string source = string.Empty;
        private string category = string.Empty;
        public Dictionary<string, string> InvestigateUrls { get; set; }

        public KrakCrawler()
        {
            InvestigateUrls = new Dictionary<string, string>();
        }

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

                var totalPages = 50; //int.Parse(pageCount.InnerHtml.Trim().Replace("... ", ""));
                var results = new List<string>();

                var parts = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                this.category = parts[2];
                if (!string.IsNullOrEmpty(category))
                {
                    this.category = this.category.Replace("%C3%A6", "æ").Replace("%C3%B8", "ø").Replace("%C3%85", "å");
                }

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
                retryCount: 2, // Retry 3 times
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

                ///profile-page-link 



                var sourceLink =
                    item.SelectSingleNode(
                        ".//a[contains(@class, 'profile-page-link')]");
                if (sourceLink?.Attributes["href"] != null)
                {
                    crawlItem.DirectUrl = "https://www.krak.dk" + sourceLink.Attributes["href"].Value;

                    if (!InvestigateUrls.ContainsKey(crawlItem.CompanyName))
                        InvestigateUrls.Add(crawlItem.CompanyName, crawlItem.DirectUrl);
                }



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

            CrawlInvestigate(result);
            CrawlWebApi(result);

            return result;
        }

        private void CrawlWebApi(CrawlerResult result)
        {
            Parallel.ForEach(result.Items.ToList(), (item) =>
            {
                var vatnumber = item.VatNumber;

                if (string.IsNullOrWhiteSpace(vatnumber))
                {
                    return;
                }

                var urlToPlayWith = "http://cvrapi.dk/api?search=" + vatnumber + "&country=dk";

                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent",
                        "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                    var data = client.DownloadString(urlToPlayWith);
                    var dto = JsonConvert.DeserializeObject<CvrApiDTO>(data);


                    var relatedResult = result.Items.FirstOrDefault(c => c.VatNumber == vatnumber);

                    if (relatedResult == null)
                    {
                        return;
                    }
                    relatedResult.CompanyEmail = dto.email;
                }
            });
        }

        private void CrawlInvestigate(CrawlerResult result)
        {
            Parallel.ForEach(InvestigateUrls.ToList(), (item) =>
            {
                try
                {
                    var relatedResult = result.Items.FirstOrDefault(c => c.CompanyName == item.Key);

                    if (relatedResult != null)
                    {
                        var url = item.Value;

                        using (var client = new WebClient())
                        {
                            var html = client.DownloadString(url);
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(html);

                            var resultBox = doc.DocumentNode.SelectSingleNode(".//dl[contains(@class, 'financial-information-facts')]");
                            var dds = resultBox.SelectNodes("//dd");

                            var hackedHtml = resultBox.InnerHtml;
                            Regex cvrRegex = new Regex("[0-9]{8}");
                            var vatnumberRegex = cvrRegex.Match(hackedHtml);

                            var data = vatnumberRegex.Value;
                            string cvr = data;
                            string employee = string.Empty;
                            if (dds.Count == 3)
                            {
                                employee = dds[2].InnerHtml;
                            }
                            if (dds.Count == 2)
                            {
                                employee = dds[1].InnerHtml;
                            }

                            relatedResult.VatNumber = cvr;
                            relatedResult.Employees = employee;
                        }
                    }
                }
                catch (Exception)
                { }


            });
            

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monera.Crawling.DGS.Domain.Models
{
    public class CrawlItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }
        public string SourceUrl { get; set; }
        public string Source { get; set; }
        public bool MarketingBlock { get; set; }
        public bool Promoted { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

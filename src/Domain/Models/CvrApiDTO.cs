using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monera.Crawling.DGS.Domain.Models
{
    public class CvrApiDTO
    {
        public int vat { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string zipcode { get; set; }
        public string city { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string startdate { get; set; }
        public string employees { get; set; }
        public int industrycode { get; set; }
        public string industrydesc { get; set; }
        public int companycode { get; set; }
        public string companydesc { get; set; }
        public bool creditbankrupt { get; set; }
        public int version { get; set; }
    }
}

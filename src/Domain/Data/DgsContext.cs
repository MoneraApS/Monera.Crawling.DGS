using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monera.Crawling.DGS.Domain.Models;

namespace Monera.Crawling.DGS.Domain.Data
{
    [DbConfigurationType(typeof(DbContextConfiguration))]
    public class DgsContext : DbContext
    {
        public DgsContext() : base("name=DgsContext")
        {
        }

        public DgsContext(string connString)
            : base(connString)
        {
        }

        public int? CommandTimeout
        {
            get { return ((IObjectContextAdapter)this).ObjectContext.CommandTimeout; }
            set { ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = value; }
        }

        public DbSet<CrawlItem> CrawlItems { get; set; }
    }
}

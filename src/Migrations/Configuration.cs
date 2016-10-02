namespace Monera.Crawling.DGS.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Monera.Crawling.DGS.Domain.Data.DgsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Monera.Crawling.DGS.Domain.Data.DgsContext context)
        {
            
        }
    }
}

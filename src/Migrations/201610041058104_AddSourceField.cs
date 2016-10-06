namespace Monera.Crawling.DGS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSourceField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CrawlItems", "Source", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CrawlItems", "Source");
        }
    }
}

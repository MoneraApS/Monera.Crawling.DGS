namespace Monera.Crawling.DGS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class morefields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CrawlItems", "VatNumber", c => c.String());
            AddColumn("dbo.CrawlItems", "Employees", c => c.String());
            AddColumn("dbo.CrawlItems", "DirectUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CrawlItems", "DirectUrl");
            DropColumn("dbo.CrawlItems", "Employees");
            DropColumn("dbo.CrawlItems", "VatNumber");
        }
    }
}

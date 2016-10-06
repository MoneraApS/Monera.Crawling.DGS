namespace Monera.Crawling.DGS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeDataType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CrawlItems", "MarketingBlock", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CrawlItems", "MarketingBlock", c => c.String());
        }
    }
}

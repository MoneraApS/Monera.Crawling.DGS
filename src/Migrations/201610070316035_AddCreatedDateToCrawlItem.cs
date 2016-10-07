namespace Monera.Crawling.DGS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCreatedDateToCrawlItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CrawlItems", "CreatedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CrawlItems", "CreatedDate");
        }
    }
}

namespace Monera.Crawling.DGS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CrawlItems", "Category", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CrawlItems", "Category");
        }
    }
}

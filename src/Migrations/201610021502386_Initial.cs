namespace Monera.Crawling.DGS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CrawlItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(),
                        CompanyPhone = c.String(),
                        CompanyAddress = c.String(),
                        CompanyEmail = c.String(),
                        SourceUrl = c.String(),
                        MarketingBlock = c.String(),
                        Promoted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CrawlItems");
        }
    }
}

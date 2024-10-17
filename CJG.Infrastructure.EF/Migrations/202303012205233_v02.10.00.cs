using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.10.00")]
    public partial class v021000 : ExtendedDbMigration
    {
	    // We need to drop these tables so we can drop the Region PK Identity and recreate them

		public override void Up()
        {
            DropForeignKey("dbo.PrioritizationPostalCodes", "RegionId", "dbo.PrioritizationRegions");
            DropIndex("dbo.PrioritizationPostalCodes", new[] { "RegionId" });
            DropTable("dbo.PrioritizationPostalCodes");
            DropTable("dbo.PrioritizationRegions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PrioritizationRegions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        RegionalScore = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PrioritizationPostalCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RegionId = c.Int(nullable: false),
                        PostalCode = c.String(),
                        DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.PrioritizationPostalCodes", "RegionId");
            AddForeignKey("dbo.PrioritizationPostalCodes", "RegionId", "dbo.PrioritizationRegions", "Id", cascadeDelete: true);
        }
    }
}

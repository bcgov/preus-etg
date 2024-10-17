using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.10.01")]
    public partial class v021001 : ExtendedDbMigration
    {
		// We need to recreate these tables so we can drop the Region PK Identity

        public override void Up()
        {
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PrioritizationRegions", t => t.RegionId, cascadeDelete: true)
                .Index(t => t.RegionId);
            
            CreateTable(
                "dbo.PrioritizationRegions",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                        RegionalScore = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);

			PostDeployment();
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PrioritizationPostalCodes", "RegionId", "dbo.PrioritizationRegions");
            DropIndex("dbo.PrioritizationPostalCodes", new[] { "RegionId" });
            DropTable("dbo.PrioritizationRegions");
            DropTable("dbo.PrioritizationPostalCodes");
        }
    }
}

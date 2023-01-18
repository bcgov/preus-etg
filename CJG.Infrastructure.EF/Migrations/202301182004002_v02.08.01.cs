using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.08.01")]
    public partial class v020801 : ExtendedDbMigration
    {
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

			PostDeployment();
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PrioritizationPostalCodes", "RegionId", "dbo.PrioritizationRegions");
            DropIndex("dbo.PrioritizationPostalCodes", new[] { "RegionId" });
            DropTable("dbo.PrioritizationPostalCodes");
        }
    }
}

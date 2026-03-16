using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.19.00")]
    public partial class v021900 : ExtendedDbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrainingObjectives",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(nullable: false, maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true)
                .Index(t => t.IsActive, name: "IX_Active");

            PostDeployment();
        }
        
        public override void Down()
        {
            DropIndex("dbo.TrainingObjectives", "IX_Active");
            DropIndex("dbo.TrainingObjectives", new[] { "Caption" });
            DropTable("dbo.TrainingObjectives");
        }
    }
}

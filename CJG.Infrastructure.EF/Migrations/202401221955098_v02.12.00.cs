using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.12.00")]
    public partial class v021200 : ExtendedDbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrainingPeriodBudgetRates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BudgetType = c.Int(nullable: false),
                        WithdrawnRate = c.Decimal(nullable: false, precision: 9, scale: 4),
                        RefusalRate = c.Decimal(nullable: false, precision: 9, scale: 4),
                        ApprovedSlippageRate = c.Decimal(nullable: false, precision: 9, scale: 4),
                        ClaimedSlippageRate = c.Decimal(nullable: false, precision: 9, scale: 4),
                        DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        TrainingPeriod_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrainingPeriods", t => t.TrainingPeriod_Id)
                .Index(t => t.TrainingPeriod_Id);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrainingPeriodBudgetRates", "TrainingPeriod_Id", "dbo.TrainingPeriods");
            DropIndex("dbo.TrainingPeriodBudgetRates", new[] { "TrainingPeriod_Id" });
            DropTable("dbo.TrainingPeriodBudgetRates");
        }
    }
}

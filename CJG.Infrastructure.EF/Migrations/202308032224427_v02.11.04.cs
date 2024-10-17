using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.11.04")]
	public partial class v021104 : ExtendedDbMigration
	{
        public override void Up()
        {
            CreateTable(
                "dbo.PrioritizationHighOpportunityOccupationScores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        NOCCode = c.String(),
                        HighOpportunityOccupationScore = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.PrioritizationScoreBreakdowns", "HighOpportunityOccupationScore", c => c.Int(nullable: false));
            AddColumn("dbo.PrioritizationScoreBreakdowns", "HighOpportunityOccupationCode", c => c.String());
            AddColumn("dbo.PrioritizationThresholds", "HighOpportunityOccupationThreshold", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PrioritizationThresholds", "HighOpportunityOccupationAssignedScore", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PrioritizationThresholds", "HighOpportunityOccupationAssignedScore");
            DropColumn("dbo.PrioritizationThresholds", "HighOpportunityOccupationThreshold");
            DropColumn("dbo.PrioritizationScoreBreakdowns", "HighOpportunityOccupationCode");
            DropColumn("dbo.PrioritizationScoreBreakdowns", "HighOpportunityOccupationScore");
            DropTable("dbo.PrioritizationHighOpportunityOccupationScores");
        }
    }
}

using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.19.06")]
	public partial class v021906 : ExtendedDbMigration
    {
		public override void Up()
        {
            AddColumn("dbo.PrioritizationScoreBreakdowns", "SkilledTradesApprenticeshipScore", c => c.Int(nullable: false));
            AddColumn("dbo.PrioritizationThresholds", "SkilledTradesApprenticeshipScore", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PrioritizationThresholds", "SkilledTradesApprenticeshipScore");
            DropColumn("dbo.PrioritizationScoreBreakdowns", "SkilledTradesApprenticeshipScore");
        }
    }
}

using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.19.05")]
	public partial class v021905 : ExtendedDbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.PrioritizationScoreBreakdowns", "PublicPostSecondaryScore", c => c.Int(nullable: false));
			AddColumn("dbo.PrioritizationThresholds", "PublicPostSecondaryScore", c => c.Int(nullable: false));
		}

		public override void Down()
		{
			DropColumn("dbo.PrioritizationThresholds", "PublicPostSecondaryScore");
			DropColumn("dbo.PrioritizationScoreBreakdowns", "PublicPostSecondaryScore");
		}
	}
}

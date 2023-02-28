using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.08.02")]
    public partial class v020802 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PrioritizationThresholds", "IndustryAssignedScore", c => c.Int(nullable: false));
            AddColumn("dbo.PrioritizationThresholds", "RegionalThresholdAssignedScore", c => c.Int(nullable: false));
            AddColumn("dbo.PrioritizationThresholds", "EmployeeCountAssignedScore", c => c.Int(nullable: false));
            AddColumn("dbo.PrioritizationThresholds", "FirstTimeApplicantAssignedScore", c => c.Int(nullable: false));

			PostDeployment();
        }
        
        public override void Down()
        {
            DropColumn("dbo.PrioritizationThresholds", "FirstTimeApplicantAssignedScore");
            DropColumn("dbo.PrioritizationThresholds", "EmployeeCountAssignedScore");
            DropColumn("dbo.PrioritizationThresholds", "RegionalThresholdAssignedScore");
            DropColumn("dbo.PrioritizationThresholds", "IndustryAssignedScore");
        }
    }
}

using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.21.01")]
    public partial class v022101 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrantApplications", "LMDAEligibilityReviewed", c => c.Boolean());
            AddColumn("dbo.ParticipantForms", "IsLMDAEligible", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParticipantForms", "IsLMDAEligible");
            DropColumn("dbo.GrantApplications", "LMDAEligibilityReviewed");
        }
    }
}

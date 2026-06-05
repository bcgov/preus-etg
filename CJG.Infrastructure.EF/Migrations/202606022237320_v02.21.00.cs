using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.21.00")]
    public partial class v022100 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParticipantForms", "EiEligibilityReportedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParticipantForms", "EiEligibilityReportedOn");
        }
    }
}

using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.06.00")]
	public partial class v020600 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrantStreamEligibilityQuestions", "EligibilityRationaleAnswerAllowed", c => c.Boolean(nullable: false));
            AddColumn("dbo.GrantStreamEligibilityQuestions", "EligibilityRationaleAnswerLabel", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrantStreamEligibilityQuestions", "EligibilityRationaleAnswerLabel");
            DropColumn("dbo.GrantStreamEligibilityQuestions", "EligibilityRationaleAnswerAllowed");
        }
    }
}

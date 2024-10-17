using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.06.01")]
	public partial class v020601 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrantStreamEligibilityAnswers", "RationaleAnswer", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrantStreamEligibilityAnswers", "RationaleAnswer");
        }
    }
}

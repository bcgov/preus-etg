using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.04.07")]
	public partial class v020507 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "ParticipantsPaidForExpenses", c => c.Boolean());
            AddColumn("dbo.Claims", "ParticipantsFullyReimbursed", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "ParticipantsFullyReimbursed");
            DropColumn("dbo.Claims", "ParticipantsPaidForExpenses");
        }
    }
}

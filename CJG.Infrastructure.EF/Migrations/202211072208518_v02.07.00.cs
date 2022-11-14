using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.06.03")]
    public partial class v020700 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "ParticipantsPaidForExpenses", c => c.Boolean());
            AddColumn("dbo.Claims", "ParticipantsHaveBeenReimbursed", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "ParticipantsHaveBeenReimbursed");
            DropColumn("dbo.Claims", "ParticipantsPaidForExpenses");
        }
    }
}

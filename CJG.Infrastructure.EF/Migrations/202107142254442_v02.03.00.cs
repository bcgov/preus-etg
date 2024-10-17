namespace CJG.Infrastructure.EF.Migrations
{
	using System.ComponentModel;

	[Description("v02.03.00")]
    public partial class v020300 : ExtendedDbMigration
	{
        public override void Up()
        {
            AddColumn("dbo.GrantStreams", "RequireAllParticipantsBeforeSubmission", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrantStreams", "RequireAllParticipantsBeforeSubmission");
        }
    }
}
